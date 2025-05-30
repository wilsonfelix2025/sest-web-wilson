using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros
{
    public class Filtro : Cálculo, IFiltro
    {
        public Filtro(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, IConversorProfundidade conversorProfundidade, ILitologia litologia) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, conversorProfundidade, litologia)
        {
        }

        public override void Execute(bool chamadaPelaPipeline)
        {
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            return PerfisEntrada.Perfis;
        }

        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            return null;
        }

        protected void AplicarLimites(IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, TipoCorteEnum tipoCorte, double limiteInferior, double limiteSuperior)
        {
            if (tipoCorte == TipoCorteEnum.Eliminar)
                AplicarLimitesEliminar(perfisEntrada, perfisSaída,limiteInferior, limiteSuperior);
            else if (tipoCorte == TipoCorteEnum.Truncar)
                AplicarLimitesTruncar(perfisEntrada, perfisSaída,limiteInferior, limiteSuperior);
        }

        private void AplicarLimitesEliminar(IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, double limiteInferior, double limiteSuperior)
        {
            var perfil = perfisSaída.Perfis.First();
            var perfilEntrada = perfisEntrada.Perfis.First();

            var novosPontos = new List<Ponto>();
            novosPontos.AddRange(perfilEntrada.GetPontos().Where(p => p.Valor >= limiteInferior && p.Valor <= limiteSuperior));

            var pms = novosPontos.Select(s => s.Pm);
            var valores = novosPontos.Select(s => s.Valor);

            perfil.AddPontosEmPm(ConversorProfundidade,pms.ToList(), valores.ToList(), TipoProfundidade.PM, OrigemPonto.Filtrado, Litologia);
        }

        private void AplicarLimitesTruncar(IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, double limiteInferior, double limiteSuperior)
        {
            var pontosDePerfilFiltradosPM = new List<Profundidade>();
            var pontosDePerfilFiltradosValor = new List<double>();
            var perfil = perfisEntrada.Perfis.First();
            var perfilSaída = perfisSaída.Perfis.First();

            foreach (var pontodePerfil in perfil.GetPontos())
            {
                double valor;

                if (pontodePerfil.Valor < limiteInferior)
                    valor = limiteInferior;
                else if (pontodePerfil.Valor > limiteSuperior)
                    valor = limiteSuperior;
                else
                    valor = pontodePerfil.Valor;

                pontosDePerfilFiltradosPM.Add(pontodePerfil.Pm);
                pontosDePerfilFiltradosValor.Add(valor);
            }

            perfilSaída.AddPontosEmPm(ConversorProfundidade, pontosDePerfilFiltradosPM, pontosDePerfilFiltradosValor, TipoProfundidade.PM,
                OrigemPonto.Filtrado, Litologia);
        }

    }
}
