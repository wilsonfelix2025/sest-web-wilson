using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
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
    public class CálculoDepleção
    {
        private PerfilBase ThorMin;
        private DepleçãoDTO Depleção;
        private IConversorProfundidade ConversorProfundidade;
        private Geometria Geometria;
        private readonly ILitologia Litologia;
        private List<PerfilBase> PerfisEntrada;
        public Dictionary<double,double> profundidadeValorThorMinSemDepleção;

        public CálculoDepleção(PerfilBase thorMin, DepleçãoDTO depleção, IConversorProfundidade conversorProfundidade, Geometria geometria, ILitologia litologia, List<PerfilBase> perfis)
        {
            ThorMin = thorMin;
            Depleção = depleção;
            ConversorProfundidade = conversorProfundidade;
            Geometria = geometria;
            Litologia = litologia;
            PerfisEntrada = perfis;
            profundidadeValorThorMinSemDepleção = new Dictionary<double, double>();
        }

        internal PerfilBase Calcular(string nome)
        {
            double profundidadeInicial = 0.0;
            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OffShore.LaminaDagua;
                    break;
                case CategoriaPoço.OnShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OnShore.AlturaDeAntePoço + Geometria.OnShore.LençolFreático;
                    break;
            }

            return CalcularDepleção(profundidadeInicial, nome);
        }

        private PerfilBase CalcularDepleção(double profundidadeInicial, string nome)
        {

            var novoThorMin = PerfisFactory.Create("THORmin", nome, ConversorProfundidade, Litologia);

            var tensãoArray = new ConcurrentBag<Ponto>();

            var gporoOrig = Depleção.GPOROOriginal;
            var gporoDepletado = Depleção.GPORODepletada;
            var poisson = Depleção.Poisson;
            var biot = Depleção.Biot;

            if (!gporoOrig.ContémPontos() || !gporoDepletado.ContémPontos() || !poisson.ContémPontos() ||
                !biot.ContémPontos())
                return null;

            var sincronizadorProfundidades = new SincronizadorProfundidades(PerfisEntrada, ConversorProfundidade, Litologia, GrupoCálculo.Tensões);
            var profundidades = sincronizadorProfundidades.GetProfundidadeDeReferência();


            ThorMin.AtualizarPvs(ConversorProfundidade);

            //Parallel.For(0, profundidades.Length, (i) =>
            for (int i = 0; i < profundidades.Length; i++)
            {
                double profundidadePM = profundidades[i];
                double profundidadePV = 0;

                if (!ConversorProfundidade.TryGetTVDFromMD(profundidadePM, out profundidadePV))
                    return null;

                if (profundidadePV < profundidadeInicial)
                    return null;

                Ponto pontoTensãoMenor, pontoGporoOrig, pontoGporoDepletado, pontoPoisson, pontoBiot;

                if (ThorMin.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePM), out pontoTensãoMenor, GrupoCálculo.Tensões) &&
                    gporoOrig.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePM), out pontoGporoOrig, GrupoCálculo.Tensões) &&
                    gporoDepletado.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePM), out pontoGporoDepletado, GrupoCálculo.Tensões) &&
                    poisson.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePM), out pontoPoisson, GrupoCálculo.Tensões) &&
                    biot.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePM), out pontoBiot, GrupoCálculo.Tensões) &&
                    Litologia.ObterGrupoLitológicoNessaProfundidade(profundidadePM, out var tipoLitologia))
                {
                    profundidadeValorThorMinSemDepleção.Add(profundidadePM, pontoTensãoMenor.Valor);

                    if (tipoLitologia == 6) //Evaporitos
                    {
                        var perfilTver = PerfisEntrada.Where(p => p.Mnemonico == "TVERT").First();
                        Ponto pontoTvertEntrada;
                        perfilTver.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePM), out pontoTvertEntrada, GrupoCálculo.Tensões);
                        var tensao = pontoTvertEntrada.Valor;
                        novoThorMin.AddPonto(ConversorProfundidade, new Profundidade(profundidadePM), new Profundidade(profundidadePV), tensao, TipoProfundidade.PM, OrigemPonto.Calculado);
                    }
                    else
                    {

                        double deltaPPoro = OperaçõesDeConversão.ObterPressão(profundidadePV, pontoGporoOrig.Valor) - OperaçõesDeConversão.ObterPressão(profundidadePV, pontoGporoDepletado.Valor);
                        double valor = ((1 - 2 * pontoPoisson.Valor) / (1 - pontoPoisson.Valor)) * pontoBiot.Valor;
                        double deltaTensão = valor * deltaPPoro;
                        double tensãoDepletada = pontoTensãoMenor.Valor - deltaTensão;                        

                        novoThorMin.AddPonto(ConversorProfundidade, new Profundidade(profundidadePM), new Profundidade(profundidadePV), tensãoDepletada, TipoProfundidade.PM, OrigemPonto.Calculado);
                    }
                }
            }
            
            return novoThorMin;
        }
    }
}