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
using SestWeb.Domain.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    public class CálculoTensãoHorizontalMenorNormalizaçãoPressãoPoros : CálculoTensãoHorizontalBase
    {
        private readonly List<PerfilBase> _entradas;
        private readonly IList<ParâmetrosLotDTO> _lotsPressãoPoros;
        private readonly IConversorProfundidade _trajetória;
        private readonly Geometria _geometria;
        private readonly ILitologia _litologia;
        private static double _coeficienteSoterramento;

        public CálculoTensãoHorizontalMenorNormalizaçãoPressãoPoros(IList<PerfilBase> entradas, IList<ParâmetrosLotDTO> lotsPressãoPoros, IConversorProfundidade trajetória, Geometria geometria, ILitologia litologia, double? coeficienteSoterramento)
            : base(entradas, trajetória, geometria, litologia)
        {
            _entradas = entradas.ToList();
            _lotsPressãoPoros = lotsPressãoPoros;
            _trajetória = trajetória;
            _geometria = geometria;
            _litologia = litologia;

            if (lotsPressãoPoros != null && lotsPressãoPoros.Any())
                _coeficienteSoterramento = CalcularCoeficienteAngular();
            else
                _coeficienteSoterramento = coeficienteSoterramento.Value;

            CorrelaçõesTensãoHorizontalMenor = MetodologiaCálculoTensãoHorizontalMenorEnum.NormalizaçãoPP;
        }
        public MetodologiaCálculoTensãoHorizontalMenorEnum CorrelaçõesTensãoHorizontalMenor { get; set; }

        public PerfilBase Calcular(string nome)
        {
            double profundidadeInicial;
            double[] profundidades = ObterProfundidades();
            switch (_geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    profundidadeInicial = _geometria.MesaRotativa + _geometria.OffShore.LaminaDagua;
                    break;
                case CategoriaPoço.OnShore:
                    profundidadeInicial = _geometria.MesaRotativa + _geometria.OnShore.AlturaDeAntePoço + _geometria.OnShore.LençolFreático;
                    break;
                default: throw new InvalidOperationException("Categoria de poço não reconhecida!");
            }

            var perfil = CalcularTHORminPorNormalizaçãoPressãoPoros(profundidadeInicial, profundidades, nome);
            return perfil;
        }

        public RetornoLotDTO GerarDadosGráfico()
        {
            var retornoLot = new RetornoLotDTO();

            for (var index = 0; index < _lotsPressãoPoros.Count; index++)
            {
                var lotPressãoPoros = _lotsPressãoPoros[index];
                var soterramento = lotPressãoPoros.ProfundidadeVertical - (lotPressãoPoros.Lda + lotPressãoPoros.MesaRotativa);
                var pressãoLot = OperaçõesDeConversão.ObterPressão(lotPressãoPoros.ProfundidadeVertical.Value, lotPressãoPoros.Lot.Value);
                var pPoro = OperaçõesDeConversão.ObterPressão(lotPressãoPoros.ProfundidadeVertical.Value,
                    lotPressãoPoros.GradPressãoPoros.Value);
                var lotMenosPporo = pressãoLot - pPoro;

                var pontoRetorno = new RetornoPontoLotDTO
                {
                    valorX = soterramento.Value,
                    valorY = lotMenosPporo
                };

                retornoLot.PontosDTO.Add(pontoRetorno);
            }

            retornoLot.Coeficiente = _coeficienteSoterramento;

            return retornoLot;
        }

        private PerfilBase CalcularTHORminPorNormalizaçãoPressãoPoros(double profundidadeInicial, double[] profundidades, string nome)
        {
            var tensãoVertical = _entradas.Single(x => x.Mnemonico == "TVERT");
            var lâminaDagua = _geometria.OffShore.LaminaDagua;
            var mesaRotativa = _geometria.MesaRotativa;

            var gradPressãoPoros = _entradas.Single(x => x.Mnemonico == "GPORO" || x.Mnemonico == "GPPI");

            var pontosThormin = new ConcurrentBag<Ponto>();

            var perfilThorMin = PerfisFactory.Create("THORmin", nome, _trajetória, _litologia);

            //TODO Paralelizar para performance
            for (int i = 0; i < profundidades.Length; i++)
            {
                var profundidadePm = profundidades[i];

                if (!_trajetória.TryGetTVDFromMD(profundidadePm, out double profundidadePv))
                    return null;
                    
                if (profundidadePv < profundidadeInicial)
                    continue;

                var soterramento = profundidadePv - (lâminaDagua + mesaRotativa);

                if (tensãoVertical.TryGetPontoEmPm(_trajetória, new Profundidade(profundidadePm), out var pontoTvert, GrupoCálculo.Tensões) 
                    && gradPressãoPoros.TryGetPontoEmPm(_trajetória, new Profundidade(profundidadePm), out var pontoGporo, GrupoCálculo.Tensões)
                    && _litologia.ObterTipoRochaNaProfundidade(profundidadePm,out var tipoLitologia))
                {
                    var thormin = CalcularTensãoHorizontalMenor(profundidadePv, soterramento, _coeficienteSoterramento, pontoGporo.Valor, pontoTvert.Valor, tipoLitologia.Grupo);
                    perfilThorMin.AddPonto(_trajetória, profundidadePm, profundidadePv, thormin, TipoProfundidade.PM, OrigemPonto.Calculado);

                }
            }
            
            return perfilThorMin;
        }

        private double CalcularCoeficienteAngular()
        {
            var soterramentos = new double[_lotsPressãoPoros.Count];
            var lots = new double[_lotsPressãoPoros.Count];

            for (var index = 0; index < _lotsPressãoPoros.Count; index++)
            {
                var lotPressãoPoros = _lotsPressãoPoros[index];
                var soterramento = lotPressãoPoros.ProfundidadeVertical - (lotPressãoPoros.Lda + lotPressãoPoros.MesaRotativa);
                var pressãoLot = OperaçõesDeConversão.ObterPressão(lotPressãoPoros.ProfundidadeVertical.Value, lotPressãoPoros.Lot.Value);
                var pPoro = OperaçõesDeConversão.ObterPressão(lotPressãoPoros.ProfundidadeVertical.Value,
                    lotPressãoPoros.GradPressãoPoros.Value);
                var lotMenosPporo = pressãoLot - pPoro;
                soterramentos[index] = soterramento.Value;
                lots[index] = lotMenosPporo;
            }

            var coeficienteAngular = CoeficienteAngular.CalcularCoeficienteAngularPassandoPelaOrigem(soterramentos, lots);
            return coeficienteAngular;
        }

        // calcula 1 ponto
        private static double CalcularTensãoHorizontalMenor(double profundidadePv, double soterramento, double coeficienteAngular, double gPoro, double tensãoVertical, GrupoLitologico grupoLitológico)
        {
            if (grupoLitológico == GrupoLitologico.Evaporitos) return tensãoVertical;

            var pPoro = OperaçõesDeConversão.ObterPressão(profundidadePv, gPoro);
            var thorMin = soterramento * coeficienteAngular + pPoro;
            return thorMin;
        }

        //public static List<EntradaCorrelação> GetEntradas()
        //{
        //    var entradas = new List<EntradaCorrelação>();
        //    var gradPressãoPoros = ModelosFixos.ObterTipoPerfil(Mnemônico.GPORO);
        //    entradas.Add(new EntradaCorrelação(gradPressãoPoros, gradPressãoPoros.Mnemônico.ToString()));
        //    return entradas;
        //}

        //public static List<ParâmetroCorrelação> GetParâmetros()
        //{
        //    return new List<ParâmetroCorrelação>();
        //}

    }
}
