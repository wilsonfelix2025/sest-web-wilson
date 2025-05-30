using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    internal class CálculoTensãoHorizontalMaior : CálculoTensãoHorizontalBase
    {
        private readonly List<PerfilBase> PerfisEntrada;
        private readonly List<PerfilBase> PerfisSaída;
        private RelaçãoTensãoDTO RelaçãoTensão;
        private readonly BreakoutDTO Breakout;
        private readonly IConversorProfundidade ConversorProfundidade;
        private readonly FraturasTrechosVerticaisDTO FraturaTrechosVerticais;
        private readonly MetodologiaCálculoTensãoHorizontalMaiorEnum MetodologiaTHORmax;
        private readonly PerfilBase GradienteTensãoMaior;
        private readonly PerfilBase AzimuthPerfil;
        private readonly PerfilBase TensãoMaior;


        public CálculoTensãoHorizontalMaior(List<PerfilBase> perfisEntrada, List<PerfilBase> perfisSaída, RelaçãoTensãoDTO parâmetrosCorrelação, BreakoutDTO breakout, IConversorProfundidade conversorProfundidade, Geometria geometria, ILitologia litologia, FraturasTrechosVerticaisDTO fraturasTrechosVerticais, MetodologiaCálculoTensãoHorizontalMaiorEnum metodologiaTHORMax)
            : base(perfisEntrada, conversorProfundidade, geometria, litologia)
        {
            PerfisEntrada = perfisEntrada;
            PerfisSaída = perfisSaída;
            RelaçãoTensão = parâmetrosCorrelação;
            Breakout = breakout;
            ConversorProfundidade = conversorProfundidade;
            FraturaTrechosVerticais = fraturasTrechosVerticais;
            MetodologiaTHORmax = metodologiaTHORMax;
            GradienteTensãoMaior = PerfisSaída.Single(p => p.Mnemonico == "GTHORmax");
            GradienteTensãoMaior.Clear();
            AzimuthPerfil = PerfisSaída.Single(p => p.Mnemonico == "AZTHmin");
            AzimuthPerfil.Clear();
            TensãoMaior = PerfisSaída.Single(p => p.Mnemonico == "THORmax");
            TensãoMaior.Clear();
        }

        public void Calcular(PerfilBase thorMinOriginal, bool depletado, DepleçãoDTO parâmetrosDepleção = null, Dictionary<double, double> profundidadeEValorThorMinSemDepleção = null)
        {
            double profundidadeInicial = 0.0;
            PerfilBase perfil;
            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OffShore.LaminaDagua;
                    break;
                case CategoriaPoço.OnShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OnShore.AlturaDeAntePoço + Geometria.OnShore.LençolFreático;
                    break;
            }

            switch (MetodologiaTHORmax)
            {
                case MetodologiaCálculoTensãoHorizontalMaiorEnum.RelaçãoEntreTensões:
                    CalcularTensãoRelação(profundidadeInicial, thorMinOriginal, depletado, parâmetrosDepleção, profundidadeEValorThorMinSemDepleção);
                    break;
                case MetodologiaCálculoTensãoHorizontalMaiorEnum.BreakoutTrechosVerticais:
                    CalcularTensãoBreakout(profundidadeInicial, thorMinOriginal, depletado, parâmetrosDepleção);
                    break;
                case MetodologiaCálculoTensãoHorizontalMaiorEnum.FraturasTrechosVerticais:
                    CalcularTensãoTrechosVerticais(profundidadeInicial, thorMinOriginal);
                    break;
                case MetodologiaCálculoTensãoHorizontalMaiorEnum.NãoEspecificado:
                default:
                    break;
            }

        }


        private void CalcularTensãoTrechosVerticais(double profundidadeInicial, PerfilBase thorMinOriginal)
        {
            var gPoro = FraturaTrechosVerticais.PerfilGPORO;
            var restr = FraturaTrechosVerticais.PerfilRESTR;
            var valorAzimuth = FraturaTrechosVerticais.Azimuth;
            var fator = 5.8674;

            foreach (var frat in FraturaTrechosVerticais.TrechosFratura)
            {
                double profundidade = frat.PM;
                double profundidadeVertical = 0.0;
                ConversorProfundidade.TryGetTVDFromMD(profundidade, out profundidadeVertical);

                if (restr.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidade), out var pontoRestr, GrupoCálculo.Tensões) &&
                    thorMinOriginal.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidade), out var pontoThorMin, GrupoCálculo.Tensões) &&
                    gPoro.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidade), out var pontoGporo, GrupoCálculo.Tensões)
                    )
                {
                    var tensão = 3 * pontoThorMin.Valor - pontoRestr.Valor - (frat.PesoFluido * profundidadeVertical / fator) - (pontoGporo.Valor * profundidadeVertical / fator);
                    var gradTensao = OperaçõesDeConversão.ObterGradiente(profundidadeVertical, tensão);

                    if (ValorPontoTHORmaxÉMenorQuePontoTHORmin(tensão, pontoThorMin.Valor))
                        tensão = pontoThorMin.Valor;

                    TensãoMaior.AddPonto(ConversorProfundidade, profundidade, profundidadeVertical, tensão, TipoProfundidade.PM, OrigemPonto.Calculado);
                    GradienteTensãoMaior.AddPonto(ConversorProfundidade, profundidade, profundidadeVertical, gradTensao, TipoProfundidade.PM, OrigemPonto.Calculado);
                    AzimuthPerfil.AddPonto(ConversorProfundidade, profundidade, profundidadeVertical, valorAzimuth, TipoProfundidade.PM, OrigemPonto.Calculado);

                }
            }

        }

        private void CalcularTensãoBreakout(double profundidadeInicial, PerfilBase thorMinOriginal, bool depletado, DepleçãoDTO parâmetrosDepleção)
        {
            var ucs = Breakout.PerfilUCS;
            var angat = Breakout.PerfilANGAT;
            var thorMin = Entradas.Last(x => x.Mnemonico == "THORmin");
            var gPoro = Breakout.PerfilGPORO;

            foreach (var breakout in Breakout.TrechosBreakout)
            {
                double profundidade = breakout.PM;

                if (ucs.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidade), out var pontoUcs, GrupoCálculo.Tensões) &&
                    thorMin.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidade), out var pontoThorMin, GrupoCálculo.Tensões) &&
                    gPoro.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidade), out var pontoGporo, GrupoCálculo.Tensões) &&
                    angat.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidade), out var pontoAngat, GrupoCálculo.Tensões)
                    )
                {
                    var gradTensão = ObterGradienteTensãoMáximaPorBreakout(profundidade,
                        pontoThorMin.Valor, pontoGporo.Valor, pontoUcs.Valor, breakout.PesoFluido, breakout.Largura, pontoAngat.Valor);

                    var tensão = OperaçõesDeConversão.ObterPressão(profundidade, gradTensão);

                    if (ValorPontoTHORmaxÉMenorQuePontoTHORmin(tensão, pontoThorMin.Valor))
                        tensão = pontoThorMin.Valor;

                    TensãoMaior.AddPontoEmPm(ConversorProfundidade, profundidade, tensão, TipoProfundidade.PM, OrigemPonto.Calculado);
                    GradienteTensãoMaior.AddPontoEmPm(ConversorProfundidade, profundidade, gradTensão, TipoProfundidade.PM, OrigemPonto.Calculado);
                    AzimuthPerfil.AddPontoEmPm(ConversorProfundidade, profundidade, breakout.Azimute, TipoProfundidade.PM, OrigemPonto.Calculado);
                }
            }
        }

        private double ObterGradienteTensãoMáximaPorBreakout(double profundidadePm, double valorThorMin, double valorGPoro,
            double valorUCS, double valorGLama, double largura, double phi)
        {
            var ucsPpg = OperaçõesDeConversão.ObterGradiente(profundidadePm, valorUCS);
            valorThorMin = OperaçõesDeConversão.ObterGradiente(profundidadePm, valorThorMin);

            var phiRadiano = phi * Math.PI / 180;
            var tetaRadiano = (90 - largura / 2) * Math.PI / 180;

            var tensão = (ucsPpg + (valorGLama - valorGPoro) * (1 + Math.Sin(phiRadiano)) /
                             (1 - Math.Sin(phiRadiano)) - valorThorMin * (1 + 2 * Math.Cos(2 * tetaRadiano)) + (valorGLama + valorGPoro)) /
                            (1 - 2 * Math.Cos(2 * tetaRadiano));

            return tensão;
        }

        private void CalcularTensãoRelação(double profundidadeInicial, PerfilBase thorMinOriginal, bool depletado, DepleçãoDTO parâmetrosDepleção = null, Dictionary<double, double> profundidadeEValorThorMinSemDepleção = null)
        {
            PerfilBase gporoDepletado = null, gPoroOriginal = null, poisson = null, biot = null;
            var gPoro = PerfisEntrada.Single(x => x.Mnemonico == "GPORO" || x.Mnemonico == "GPPI");
            var thorMinMax = PerfisEntrada.Single(x => x.Mnemonico == "RET");
            var tensãoVertical = RelaçãoTensão.PerfilTVERT;

            if (depletado)
            {
                gporoDepletado = parâmetrosDepleção.GPORODepletada;
                gPoroOriginal = parâmetrosDepleção.GPOROOriginal;
                poisson = parâmetrosDepleção.Poisson;
                biot = parâmetrosDepleção.Biot;
            }

            var profundidades = ObterProfundidades();
            var tensãoMaior = new ConcurrentBag<Ponto>();
            var gradienteTensãoMaior = new ConcurrentBag<Ponto>();
            var azimuth = new ConcurrentBag<Ponto>();

            var valorAzimuth = RelaçãoTensão.AZTHMenor;


            //TODO Parallel.For(0, profundidades.Length, i =>

            for (int i = 0; i < profundidades.Length; i++)
            {
                var profundidadePm = profundidades[i];
                double profundidadePv = 0;

                if (!ConversorProfundidade.TryGetTVDFromMD(profundidadePm, out profundidadePv))
                    break;

                if (profundidadePv < profundidadeInicial)
                    break;

                if (thorMinOriginal.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out var pontoThorMinOriginal, GrupoCálculo.Tensões) &&
                    gPoro.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out var pontoGporo, GrupoCálculo.Tensões) &&
                    thorMinMax.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out var pontoThorMinMax, GrupoCálculo.Tensões) &&
                    Litologia.ObterGrupoLitológicoNessaProfundidade(profundidadePm, out var tipoLitologia))
                {
                    Ponto pontoGporoDepletado = null, pontoPoisson = null, pontoBiot = null, pontoGporoOriginal = null;
                    if (depletado)
                    {
                        gporoDepletado.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out pontoGporoDepletado, GrupoCálculo.Tensões);
                        poisson.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out pontoPoisson, GrupoCálculo.Tensões);
                        biot.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out pontoBiot, GrupoCálculo.Tensões);
                        gPoroOriginal.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out pontoGporoOriginal, GrupoCálculo.Tensões);
                    }

                    var grupoLitológico = tipoLitologia;

                    Ponto pontoTvert = null;
                    tensãoVertical.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out pontoTvert, GrupoCálculo.Tensões);

                    double tensao = 0.0;

                    if (grupoLitológico == 6) //Evaporitos
                    {
                        if (pontoTvert != null)
                            tensao = pontoTvert.Valor;
                        else
                        {
                            var perfilTver = PerfisEntrada.Where(p => p.Mnemonico == "TVERT").First();
                            Ponto pontoTvertEntrada;
                            perfilTver.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out pontoTvertEntrada, GrupoCálculo.Tensões);
                            tensao = pontoTvertEntrada.Valor;
                        }

                    }
                    else
                    {
                        if (depletado)
                        {
                            tensao = ObterTensãoMáximaComDepleção(profundidadePv, pontoThorMinMax.Valor, grupoLitológico, pontoThorMinOriginal.Valor, pontoGporoOriginal.Valor, pontoGporoDepletado.Valor, pontoPoisson.Valor, pontoBiot.Valor, profundidadePm, profundidadeEValorThorMinSemDepleção);
                        }
                        else
                        {

                            switch (RelaçãoTensão.TipoRelação)
                            {
                                case TipoRelaçãoEntreTensãoEnum.THORmLinhaXTHORMLinha:
                                    tensao = ObterTensãoMáxima(profundidadePv, pontoGporo.Valor, pontoThorMinMax.Valor, grupoLitológico, pontoThorMinOriginal.Valor);
                                    break;
                                case TipoRelaçãoEntreTensãoEnum.THORmXTHORM:
                                    tensao = ObterTensãoMáxima2(profundidadePv, pontoThorMinMax.Valor, pontoThorMinOriginal.Valor);
                                    break;
                                case TipoRelaçãoEntreTensãoEnum.THORMLinhaXTVertLinha:
                                    tensao = ObterTensãoMáximaTvert(profundidadePv, pontoGporo.Valor, pontoThorMinMax.Valor, pontoTvert.Valor, grupoLitológico, pontoThorMinOriginal.Valor);
                                    break;
                                case TipoRelaçãoEntreTensãoEnum.THORMXTVert:
                                    tensao = ObterTensãoMáxima2Tvert(profundidadePv, pontoThorMinMax.Valor, pontoThorMinOriginal.Valor, pontoTvert.Valor);
                                    break;
                                case TipoRelaçãoEntreTensãoEnum.NãoEspecificado:
                                default:
                                    break;
                            }
                        }
                    }
                    if (ValorPontoTHORmaxÉMenorQuePontoTHORmin(tensao, pontoThorMinOriginal.Valor))
                        tensao = pontoThorMinOriginal.Valor;

                    var gradTensao = OperaçõesDeConversão.ObterGradiente(profundidadePv, tensao);

                    TensãoMaior.AddPonto(ConversorProfundidade, profundidadePm, profundidadePv, tensao, TipoProfundidade.PM, OrigemPonto.Calculado);
                    GradienteTensãoMaior.AddPonto(ConversorProfundidade, profundidadePm, profundidadePv, gradTensao, TipoProfundidade.PM, OrigemPonto.Calculado);
                    AzimuthPerfil.AddPonto(ConversorProfundidade, profundidadePm, profundidadePv, valorAzimuth.Value, TipoProfundidade.PM, OrigemPonto.Calculado);
                }
            }

        }

        private bool ValorPontoTHORmaxÉMenorQuePontoTHORmin(double tensao, double valorThorMin)
        {
            return tensao < valorThorMin;
        }

        private double ObterTensãoMáxima2(double profundidadePv, double tensãoMinMax, double tensãoMinOriginal)
        {
            var tensãoMaxE = 0.0;

            tensãoMaxE = tensãoMinOriginal / tensãoMinMax;

            return tensãoMaxE;
        }

        private double ObterTensãoMáxima2Tvert(double profundidadePv, double tensãoMinMax, double tensãoMinOriginal, double tvert)
        {
            var tensãoMaxE = 0.0;

            tensãoMaxE = tvert * tensãoMinMax;

            return tensãoMaxE;
        }

        // não depletado
        private double ObterTensãoMáxima(double profundidadePv, double gPoro, double tensãoMinMax,
            int grupoLitológico, double tensãoMinOriginal)
        {
            var pPoro = OperaçõesDeConversão.ObterPressão(profundidadePv, gPoro);

            var tensãoMinE = tensãoMinOriginal - pPoro;
            var tensãoMaxE = tensãoMinE / tensãoMinMax;
            return tensãoMaxE + pPoro;

        }

        private double ObterTensãoMáximaTvert(double profundidadePv, double gPoro, double tensãoMinMax,
            double tvert, int grupoLitológico, double tensãoMinOriginal)
        {
            if (grupoLitológico == 6) //Evaporitos
                return tvert;

            var pPoro = OperaçõesDeConversão.ObterPressão(profundidadePv, gPoro);

            var tensão1 = tvert - pPoro;
            var tensãoMaxE = tensão1 * tensãoMinMax;
            return tensãoMaxE + pPoro;

        }

        // depletado
        private double ObterTensãoMáximaComDepleção(double profundidadePv, double tensãoMinMax,
            int grupoLitológico, double tensãoMinOriginal, double gPoroOriginal, double gPorodepletado, double poiss
            , double biot, double profundidadePm, Dictionary<double, double> profundidadeEValorThorMinSemDepleção)
        {
            if (profundidadeEValorThorMinSemDepleção == null)
                throw new Exception("Erro ao buscar valor de ThorMin sem depleção");

            var achouValorThorMinSemDepleção = profundidadeEValorThorMinSemDepleção.TryGetValue(profundidadePm, out double valorThorMinSemDepleção);

            var pPoro = OperaçõesDeConversão.ObterPressão(profundidadePv, gPoroOriginal);
            var tensãoMinE = valorThorMinSemDepleção - pPoro;
            var tensãoMaxE = tensãoMinE / tensãoMinMax;

            var tensaoMaxOriginal = tensãoMaxE + pPoro;

            var ppOriginal = OperaçõesDeConversão.ObterPressão(profundidadePv, gPoroOriginal);
            var ppDepletado = OperaçõesDeConversão.ObterPressão(profundidadePv, gPorodepletado);
            double deltaPPoro = ppOriginal - ppDepletado;
            double valor = ((1 - 2 * poiss) / (1 - poiss)) * biot;
            double deltaTensão = valor * deltaPPoro;
            double tensãoDepletada = tensaoMaxOriginal - deltaTensão;

            return tensãoDepletada;
        }

    }
}