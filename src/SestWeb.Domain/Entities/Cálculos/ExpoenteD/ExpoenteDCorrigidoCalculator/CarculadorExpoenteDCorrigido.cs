using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.ExpoenteDCalulator;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.ExpoenteDCorrigidoCalculator
{
    /// <summary>
    /// Classe para realizar o cálculo de expoente D corrigido.
    /// </summary>
    internal class CarculadorExpoenteDCorrigido : CalculadorExpoenteD
    {
        private const double Pnormal = 8.5;

        /// <summary>
        /// Calcula expoente D corrigido.
        /// </summary>
        /// <param name="entradas">Lista de perfis para realizar o cálculo.</param>
        /// <param name="perfilSaída">Perfil expoente D gerado.</param>
        internal override void Calcular(IPerfisEntrada entradas, ref Entities.Perfis.TiposPerfil.ExpoenteD perfilSaída, ILitologia litologia, IConversorProfundidade trajetória)
        {
            var perfilExpoenteD = (Entities.Perfis.TiposPerfil.ExpoenteD)PerfisFactory.Create("ExpoenteD", "ExpoenteD", trajetória, litologia);
            base.Calcular(entradas, ref perfilExpoenteD, litologia, trajetória);
            var perfilEcd = entradas.Perfis.Single(p => p.Mnemonico == "GECD");

            var sincronizadorDeProfundidades = new SincronizadorProfundidades(entradas.Perfis, trajetória, litologia, GrupoCálculo.ExpoenteD);
            var profundidades = sincronizadorDeProfundidades.GetProfundidadeDeReferência();

            foreach (var prof in profundidades)
            {
                var profundidade = new Profundidade(prof);

                if (!perfilEcd.TryGetPontoEmPm(trajetória, profundidade, out var pontoEcd, GrupoCálculo.ExpoenteDCorrigido)) 
                    continue;

                if (!perfilExpoenteD.TryGetPontoEmPm(trajetória, profundidade, out var pontoExpoenteD, GrupoCálculo.ExpoenteDCorrigido)) 
                    continue;

                var expoenteCorrigido = ObterExpoente(pontoExpoenteD.Valor, pontoEcd.Valor);
                perfilSaída.AddPontoEmPm(trajetória,profundidade, expoenteCorrigido,TipoProfundidade.PM,OrigemPonto.Calculado);
            }
        }

        private double ObterExpoente(double d, double ecd)
        {
            return d * (Pnormal / ecd);
        }
    }
}
