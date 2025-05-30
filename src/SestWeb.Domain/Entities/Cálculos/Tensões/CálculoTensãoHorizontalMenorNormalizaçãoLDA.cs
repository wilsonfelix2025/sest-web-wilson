using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    public class CálculoTensãoHorizontalMenorNormalizaçãoLDA
    {
        private const double Constante = 1.4223;
        private readonly DadosGerais DadosGerais;
        private readonly ILitologia Litologia;
        private static double CoeficienteSoterramento;
        private readonly PerfilBase PerfilEntrada;
        private readonly Geometria Geometria;
        private readonly IList<ParâmetrosLotDTO> LotsLda;
        private readonly IConversorProfundidade Trajetória;

        public CálculoTensãoHorizontalMenorNormalizaçãoLDA(PerfilBase entrada,
            IList<ParâmetrosLotDTO> lotsLda, IConversorProfundidade trajetória, Geometria geometria,
            DadosGerais dadosGerais, ILitologia litologia, double? coeficienteSoterramento)
        {
            PerfilEntrada = entrada;
            LotsLda = lotsLda;
            Trajetória = trajetória;
            Geometria = geometria;
            DadosGerais = dadosGerais;
            Litologia = litologia;

            if (LotsLda != null && LotsLda.Any())
                CoeficienteSoterramento = CalcularCoeficienteAngular(DadosGerais.Area.DensidadeAguaMar);
            else
                CoeficienteSoterramento = coeficienteSoterramento.Value;
        }

        public RetornoLotDTO GerarDadosGráfico()
        {
            var retornoLot = new RetornoLotDTO();

            for (var index = 0; index < LotsLda.Count; index++)
            {
                var lotLda = LotsLda[index];
                var soterramento = lotLda.ProfundidadeVertical - (lotLda.Lda + lotLda.MesaRotativa);
                var pressãoLda = Constante * DadosGerais.Area.DensidadeAguaMar * lotLda.Lda;
                var pressãoFluido = OperaçõesDeConversão.ObterPressão(lotLda.ProfundidadeVertical.Value, lotLda.Lot.Value);
                var lotMenosLda = pressãoFluido - pressãoLda;

                var pontoRetorno = new RetornoPontoLotDTO
                {
                    valorX = soterramento.Value,
                    valorY = lotMenosLda.Value
                };

                retornoLot.PontosDTO.Add(pontoRetorno);
            }

            retornoLot.Coeficiente = CoeficienteSoterramento;

            return retornoLot;
        }

        public PerfilBase Calcular(double[] profundidades, string nome)
        {
            double profundidadeInicial;
            var perfilThorMin = PerfisFactory.Create("THORmin", nome, Trajetória, Litologia);


            if (Geometria.CategoriaPoço == CategoriaPoço.OffShore)
                profundidadeInicial = Geometria.MesaRotativa + Geometria.OffShore.LaminaDagua;
            else
                profundidadeInicial = 0.0; 
            
            var tensãoVertical = PerfilEntrada;
            var densidadeDagua = DadosGerais.Area.DensidadeAguaMar;
            var lâminaDagua = Geometria.OffShore.LaminaDagua;
            var mesaRotativa = Geometria.MesaRotativa;

            var pontosThormin = new ConcurrentBag<Ponto>();

            //TODO Paralelizar para performance
            for (int index = 0; index < profundidades.Length; index++)
            {
                var profundidadePm = profundidades[index];

                if (!Trajetória.TryGetTVDFromMD(profundidadePm, out double profundidadePv))
                    return null;

                if (profundidadePv < profundidadeInicial)
                    continue;

                var soterramento = profundidadePv - (lâminaDagua + mesaRotativa);

                if (tensãoVertical.TryGetPontoEmPm(Trajetória, new Profundidade(profundidadePm), out var pontoTVert, GrupoCálculo.Tensões) &&
                    Litologia.ObterTipoRochaNaProfundidade(profundidadePm, out var tipoLitologia))
                {
                    var thormin = CalcularTensãoHorizontalMenor(profundidadePv, densidadeDagua, lâminaDagua,
                        CoeficienteSoterramento, soterramento, pontoTVert.Valor, tipoLitologia.Grupo);                

                    perfilThorMin.AddPonto(Trajetória, profundidadePm, profundidadePv, thormin, TipoProfundidade.PM, OrigemPonto.Calculado);                    
                }
            }
           
            return perfilThorMin;
        }

        private double CalcularTensãoHorizontalMenor(double profundidadePv, double densidadeÁgua,
            double lâminaDagua, double coeficienteAngular, double soterramento, double tvert, GrupoLitologico grupoLitológico)
        {
            if (grupoLitológico == GrupoLitologico.Evaporitos) return tvert;
            var thorMin = Constante * densidadeÁgua * lâminaDagua + coeficienteAngular * soterramento;
            return thorMin;
        }

        private double CalcularCoeficienteAngular(double densidadeDagua)
        {
            var soterramentos = new double[LotsLda.Count];
            var lots = new double[LotsLda.Count];

            for (var index = 0; index < LotsLda.Count; index++)
            {
                var lotLda = LotsLda[index];
                var soterramento = lotLda.ProfundidadeVertical - (lotLda.Lda + lotLda.MesaRotativa);
                var pressãoLda = Constante * densidadeDagua * lotLda.Lda;
                var pressãoFluido = OperaçõesDeConversão.ObterPressão(lotLda.ProfundidadeVertical.Value, lotLda.Lot.Value);
                var lotMenosLda = pressãoFluido - pressãoLda;

                soterramentos[index] = soterramento.Value;
                lots[index] = lotMenosLda.Value;
            }

            return CoeficienteAngular.CalcularCoeficienteAngularPassandoPelaOrigem(soterramentos, lots);
        }
    }
}
