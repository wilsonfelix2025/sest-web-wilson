using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.ExpoenteDCalulator
{
    /// <summary>
    /// Classe para realizar o cálculo de expoente D.
    /// </summary>
    internal class CalculadorExpoenteD
    {
        /// <summary>
        /// Calcula expoente D.
        /// </summary>
        /// <param name="entradas">Perfis de entrada do cálculo de ExpoenteD.</param>
        /// <param name="perfilSaída">Perfil expoente D gerado.</param>
        internal virtual void Calcular(IPerfisEntrada entradas, ref Entities.Perfis.TiposPerfil.ExpoenteD perfilSaída, ILitologia litologia, IConversorProfundidade trajetória)
        {
            var perfilROP = entradas.Perfis.Any(p => p.Mnemonico == "ROP")
                ? entradas.Perfis.Single(p => p.Mnemonico == "ROP")
                : ConverterIROPParaROP(entradas.Perfis.Single(p => p.Mnemonico == "IROP"), trajetória);

            var perfilRPM = entradas.Perfis.Single(p => p.Mnemonico == "RPM");
            var perfilWOB = entradas.Perfis.Single(p => p.Mnemonico == "WOB");
            var perfilDiametroBroca = entradas.Perfis.Single(p => p.Mnemonico == "DIAM_BROCA");

            var sincronizadorDeProfundidades = new SincronizadorProfundidades(entradas.Perfis, trajetória, litologia, GrupoCálculo.ExpoenteD);
            var profundidades = sincronizadorDeProfundidades.GetProfundidadeDeReferência();

            var tuplas = new ConcurrentBag<Tuple<double, double>>();

            Parallel.For(0, profundidades.Length, (i) =>
            {
                var profundidade = new Profundidade(profundidades[i]);

                Ponto pontoRop;
                Ponto pontoRPM;
                Ponto pontoWOB;
                Ponto pontoDiametroBroca;

                if (perfilROP.TryGetPontoEmPm(trajetória, profundidade, out pontoRop, GrupoCálculo.ExpoenteD) &&
                    perfilRPM.TryGetPontoEmPm(trajetória, profundidade, out pontoRPM, GrupoCálculo.ExpoenteD) &&
                    perfilWOB.TryGetPontoEmPm(trajetória, profundidade, out pontoWOB, GrupoCálculo.ExpoenteD) &&
                    perfilDiametroBroca.TryGetPontoOrPreviousEmPm(trajetória, profundidade, out pontoDiametroBroca) &&
                    litologia.ObterTipoRochaNaProfundidade(profundidades[i], out var tipoLitologia))
                {
                    var grupoLitológico = tipoLitologia.Grupo;
                    double ropValor = pontoRop.Valor;

                    var expoente = ObterExpoente(ropValor, pontoRPM.Valor, pontoWOB.Valor, pontoDiametroBroca.Valor, grupoLitológico);
                    if (!double.IsInfinity(expoente) && !double.IsNaN(expoente))
                        tuplas.Add(new Tuple<double, double>(profundidade.Valor, expoente));
                }
            });

            foreach (var (item1, item2) in tuplas)
            {
                perfilSaída.AddPontoEmPm(trajetória, new Profundidade(item1), item2,TipoProfundidade.PM,OrigemPonto.Calculado, litologia);
            }
        }

        /// <summary>
        /// Converte perfil IROP para ROP.
        /// </summary>
        /// <param name="perfil">Perfil IROP</param>
        /// <returns>Retorna o perfil ROP gerado.</returns>
        private PerfilBase ConverterIROPParaROP(PerfilBase perfil, IConversorProfundidade trajetória)
        {
            if (perfil == null || perfil.Mnemonico != "IROP")
                return null;

            var perfilROP = PerfisFactory.Create(perfil.Mnemonico, perfil.Nome, trajetória, perfil.Litologia);
            var unidade = GrupoUnidades.GetUnidadePadrão(perfil.Mnemonico);
            var grupo = GrupoUnidades.GetGrupoUnidades(perfil.Mnemonico);

            foreach (var ponto in perfil.GetPontos())
            {
                //perfilROP.AddPontoEmPm(trajetória, ponto.Profundidade, grupo.ConvertToUnidadePadrão(unidade, ponto.Valor), TipoProfundidade.PM, OrigemPonto.Editado);
                perfilROP.AddPontoEmPm(trajetória, ponto.Profundidade, 60/ponto.Valor, TipoProfundidade.PM, OrigemPonto.Editado);
            }

            return perfilROP;
        }

        internal double ObterExpoente(double ropValor, double rpmValor, double wobValor, double diametroBrocaValor, GrupoLitologico grupoLitológico)
        {
            if (grupoLitológico == GrupoLitologico.Evaporitos) return 0.0;
            return Math.Log10(ropValor / (60 * rpmValor)) / Math.Log10((12 * wobValor) / ((1000) * diametroBrocaValor));
        }
    }
}
