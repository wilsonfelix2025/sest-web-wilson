using MathNet.Numerics;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base
{
    public abstract class CálculoPressãoPorosBase: Cálculo
    {
        protected List<ParâmetroCorrelação> ParâmetrosCorrelação { get; }
        protected Geometria Geometria { get; }
        protected Trajetória Trajetória { get; }
        public CorrelaçãoPressãoPoros MétodoCálculo { get; private set; }

        public const double PressãoAtmosférica = 14.6959488;
        public const double FatorConversão = 0.1704;

        public double Gn { get; set; }
        public double Exp { get; set; }
        public double Gpph { get; set; }

        internal CálculoPressãoPorosBase(string nome, GrupoCálculo grupoCálculo, CorrelaçãoPressãoPoros correlaçãoPressãoPoros,
            IPerfisEntrada entradas, IPerfisSaída saídas, IList<ParâmetroCorrelação> parâmetrosCorrelação,
            Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais): base(nome, grupoCálculo, entradas, saídas, trajetória, litologia)
        {
            MétodoCálculo = correlaçãoPressãoPoros;
            ParâmetrosCorrelação = parâmetrosCorrelação.ToList();
            Geometria = dadosGerais.Geometria;
            Trajetória = trajetória;

            var gnCorrelaçao = ParâmetrosCorrelação.SingleOrDefault(x => x.NomeParâmetro == nameof(Gn));
            Gn = gnCorrelaçao == null ? 0 : gnCorrelaçao.Valor;

            var expCorrelaçao = ParâmetrosCorrelação.SingleOrDefault(x => x.NomeParâmetro == nameof(Exp));
            Exp = expCorrelaçao == null ? 0 : expCorrelaçao.Valor;
        }

        #region Métodos abstratos

        protected abstract double CalcularGporo(double profundidadePm, double profundidadePv, double valorSobrecarga, double valor, double valorPerfil, double exp, Func<double, double> getGppn, GrupoLitologico nomeLitologia, ConcurrentBag<Tuple<double, double>> profundidadesForaLitologia);

        /// <summary>
        /// Obtem perfil usado em cada tipo de cálculo (DTC, ExpoenteD...).
        /// </summary>
        /// <returns>Retorna o perfil observado.</returns>
        public abstract PerfilBase ObterPerfilObservado();

        /// <summary>
        /// Obtem perfil filtrado com trend.
        /// </summary>
        /// <returns>Retorna perfil com trend.</returns>
        public abstract PerfilBase ObterPerfilComTrend();

        #endregion Métodos abstratos

        protected virtual IList<Ponto> Calcular(double profundidadeInicial, Func<double, double> getGppn, Func<double, double> getHidrostática)
        {
            var pontos = new ConcurrentBag<Ponto>();
            object initLock = new object();
            var profundidades = ObterProfundidades();
            var sobrecarga = PerfisEntrada.Perfis.Single(x => x.Mnemonico == TiposPerfil.GeTipoPerfil<GSOBR>().Mnemônico);
            var perfil = ObterPerfilObservado();
            var perfilComTrend = ObterPerfilComTrend();
            var trend = perfilComTrend.Trend;
            var pontosDaLitologia = profundidades.Where(x => Litologia.ObterTipoRochaNaProfundidade(x, out var tipoRocha) == true && tipoRocha.Grupo.Equals(GrupoLitologico.Ígneas)).ToList();

            //if (trend.Pontos.Any() &&
            //    profundidades.Any() &&
            //    trend.Pontos.Last().Profundidade < profundidades.Last())
            //{
            //    var atualizouTrend = AtualizarTrend(trend, profundidades);
            //    if (!atualizouTrend)
            //        return resultado.ToArray(); 
            //}

            double exp = ParâmetrosCorrelação.Single(x => x.NomeParâmetro == nameof(Exp)).Valor;
            bool começouCalcular = false;
            var profundidadesForaLitologia = new ConcurrentBag<Tuple<double, double>>();
            double ultimaProfundidadePmCalculada = 0.0;

            for (var index = 0; index < profundidades.Length; index++)
            //Parallel.For(0, profundidades.Length, index =>
            {
                var profundidadePm = profundidades[index];
                double profundidadePv = 0.0;

                if (profundidadePm < profundidadeInicial)
                {
                    Console.WriteLine($"Continuando pq {profundidadePm} < {profundidadeInicial}");
                    continue;
                }

                if (!Trajetória.TryGetTVDFromMD(profundidadePm, out profundidadePv))
                {
                    Console.WriteLine($"Continuando pq não foi possível obter PV no PM {profundidadePm}");
                    continue;
                }

                if (Litologia.ObterTipoRochaNaProfundidade(profundidadePm, out var tipoRocha))
                {
                    if (tipoRocha.Grupo.Equals(GrupoLitologico.Evaporitos) ||
                        tipoRocha.Grupo.Equals(GrupoLitologico.Ígneas))
                    {
                        var gporo = 0.0;
                        pontos.Add(new Ponto(
                            new Profundidade(profundidadePm), new Profundidade(profundidadePv), gporo, TipoProfundidade.PM, OrigemPonto.Calculado, tipoRocha.Mnemonico));

                        lock (initLock)
                        {
                            if (!começouCalcular)
                            {
                                começouCalcular = true;
                            }
                        }

                        ultimaProfundidadePmCalculada = profundidadePm;
                        Console.WriteLine($"Continuando pq litologia no PM {profundidadePm} é evaporito ou ígnea");
                        continue;
                    }
                }

                if (perfilComTrend.TryGetPontoEmPv(ConversorProfundidade, perfilComTrend, new Profundidade(profundidadePv), out _))
                {
                    if (
                        trend.TryGetValor(new Profundidade(profundidadePv), out var valorTrend) &&
                        sobrecarga.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out var pontoSobrecarga, GrupoCálculo) &&
                        perfil.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out var pontoPerfil, GrupoCálculo) &&
                        pontoPerfil.Origem != OrigemPonto.Interpolado) // &&
                                                          //Litologia.TryGetTipoLitologia(new Profundidade(profundidadePv), TipoProfundidade.PV, out var tipoLitologia))
                    {
                        var gporo = CalcularGporo(profundidadePm, profundidadePv, pontoSobrecarga.Valor, valorTrend,
                            pontoPerfil.Valor, exp, getGppn, tipoRocha.Grupo, profundidadesForaLitologia);

                        pontos.Add(new Ponto(
                            new Profundidade(profundidadePm), new Profundidade(profundidadePv), gporo, TipoProfundidade.PM, OrigemPonto.Calculado, tipoRocha.Mnemonico));

                        lock (initLock)
                        {
                            if (!começouCalcular)
                            {
                                começouCalcular = true;
                            }
                        }

                        ultimaProfundidadePmCalculada = profundidadePm;
                    } else {
                        Console.WriteLine($"{trend.TryGetValor(new Profundidade(profundidadePv), out var _1)}, {sobrecarga.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out var _2, GrupoCálculo)}, {perfil.TryGetPontoEmPm(ConversorProfundidade, new Profundidade(profundidadePm), out var _3, GrupoCálculo)}");
                    }
                }
                else
                {
                    if (!começouCalcular)
                    {
                        pontos.Add(new Ponto(new Profundidade(profundidadePm), new Profundidade(profundidadePv), getHidrostática(profundidadePv),
                            TipoProfundidade.PM, OrigemPonto.Calculado, tipoRocha.Mnemonico));
                    }
                    else if (profundidadePm <= Litologia.UltimoPonto.Pm.Valor)
                    {
                        profundidadesForaLitologia.Add(new Tuple<double, double>(profundidadePm, profundidadePv));
                    }
                    else
                    {
                        Console.WriteLine($"Continuando pq perfil com trend nao tem PV {profundidadePv}");
                    }
                }
            }

            var interpolator = Interpolate.Linear(pontos.Select(x => x.Pm.Valor), pontos.Select(x => x.Valor));

            Parallel.ForEach(profundidadesForaLitologia, profsForaLitologia =>
            {
                var pm = profsForaLitologia.Item1;
                var pv = profsForaLitologia.Item2;
                var result = Litologia.TryGetLitoPontoEmPm(Trajetória, new Profundidade(pm), out var pontoLitologia);
                var mnemônico = pontoLitologia != null ? pontoLitologia.TipoRocha.Mnemonico : "";

                if (pm > ultimaProfundidadePmCalculada)
                {
                    pontos.Add(new Ponto(new Profundidade(pm), new Profundidade(pv), getHidrostática(pv), TipoProfundidade.PM, OrigemPonto.Calculado, mnemônico));
                }
                else
                {
                    var valor = interpolator.Interpolate(pm);
                    pontos.Add(new Ponto(new Profundidade(pm), new Profundidade(pv), valor, TipoProfundidade.PM, OrigemPonto.Calculado, mnemônico));
                }
            });

            return pontos.ToList();
        }

        public virtual IList<Ponto> Calcular()
        {
            double profundidadeInicial;
            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OffShore.LaminaDagua;
                    return Calcular(profundidadeInicial, CalcularGppnOffShore, GetHidrostáticaOffShore);
                case CategoriaPoço.OnShore:
                    profundidadeInicial = Geometria.MesaRotativa + Geometria.OnShore.AlturaDeAntePoço + Geometria.OnShore.LençolFreático;
                    return Calcular(profundidadeInicial, CalcularGppnOnShore, GetHidrostáticaOnShore);

                default: throw new InvalidOperationException("Categoria de poço não reconhecida!");
            }
        }

        protected double CalcularGppnOffShore(double profundidadeVertical)
        {
            double gn = ParâmetrosCorrelação.Single(x => x.NomeParâmetro == nameof(Gn)).Valor;
            double pPn = gn * (profundidadeVertical - Geometria.MesaRotativa) * FatorConversão + PressãoAtmosférica;
            double gppn = pPn / (FatorConversão * profundidadeVertical);
            return gppn;
        }

        protected double CalcularGppnOnShore(double profundidadeVertical)
        {
            double gn = ParâmetrosCorrelação.Single(x => x.NomeParâmetro == nameof(Gn)).Valor;
            double pPn = gn * (profundidadeVertical - (Geometria.MesaRotativa + Geometria.OnShore.LençolFreático + Geometria.OnShore.AlturaDeAntePoço)) * FatorConversão + PressãoAtmosférica;
            double gppn = pPn / (FatorConversão * profundidadeVertical);
            return gppn;
        }

        protected double[] ObterProfundidades()
        {
            var entradasSemPerfilFiltrado = new List<PerfilBase>();
            var dtcFiltrado = ObterPerfilComTrend();

            if (PerfisEntrada.ContémPerfil(dtcFiltrado.Mnemonico))
            {
                foreach (var entrada in PerfisEntrada.Perfis)
                {
                    if (entrada != dtcFiltrado)
                    {
                        entradasSemPerfilFiltrado.Add(entrada);
                    }
                }
            }

            var perfilDummyLitologia = PerfisFactory.Create("GPPI", "Litologia", Trajetória, Litologia);
            for (var i = 0; i < Litologia.Pontos.Count(); i++)
            {
                var pontoLitologia = Litologia.Pontos[i];
                perfilDummyLitologia.AddPontoEmPm(Trajetória, pontoLitologia.Pm, pontoLitologia.Valor, TipoProfundidade.PM, OrigemPonto.Calculado);

                if (i > 0)
                {
                    perfilDummyLitologia.AddPontoEmPm(Trajetória, new Profundidade(pontoLitologia.Pm.Valor - 0.1), pontoLitologia.Valor, TipoProfundidade.PM, OrigemPonto.Calculado);
                }
            }
            entradasSemPerfilFiltrado.Add(perfilDummyLitologia);
            
            var sincronizadorProfundidades = new SincronizadorProfundidades(entradasSemPerfilFiltrado, ConversorProfundidade, Litologia, GrupoCálculo);
            return sincronizadorProfundidades.GetProfundidadeDeReferência();
        }

        protected double GetHidrostáticaOffShore(double profundidade)
        {
            var gn = ParâmetrosCorrelação.Single(x => x.NomeParâmetro == nameof(Gn)).Valor;
            double ppn = gn * (profundidade - Geometria.MesaRotativa) * FatorConversão;
            ppn += PressãoAtmosférica;

            return ppn / (FatorConversão * profundidade);
        }

        protected double GetHidrostáticaOnShore(double profundidade)
        {
            var gn = ParâmetrosCorrelação.Single(x => x.NomeParâmetro == nameof(Gn)).Valor;
            double ppn = gn * (profundidade - (Geometria.MesaRotativa + Geometria.OnShore.LençolFreático + Geometria.OnShore.AlturaDeAntePoço)) * FatorConversão;
            ppn += PressãoAtmosférica;

            return ppn / (FatorConversão * profundidade);
        }
    }
}
