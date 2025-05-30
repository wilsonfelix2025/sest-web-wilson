using System;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Linq;
using SestWeb.Domain.Validadores;

namespace SestWeb.Domain.ComposiçãoPerfil
{

    public class CompositorDePerfil
    {
        protected internal Poço Poço;
        protected internal PerfilBase Perfil;

        public CompositorDePerfil(Poço poço, PerfilBase PerfilBase)
        {
            Poço = poço;

            Perfil = PerfilBase;
        }

        public void AdicionarTrecho(PerfilBase perfil, double pmTopo, double pmBase)
        {
            var profPmTopo = new Profundidade(pmTopo);
            var profPmBase = new Profundidade(pmBase);

            var achouPontosPm = perfil.TryGetPontosEmPm(profPmTopo, profPmBase, out var pontosPmTrecho);

            var pontos = pontosPmTrecho.ToList();

            if (!pontos.Any())
                return;

            for (var pointIndex = 0; pointIndex < pontos.Count; pointIndex++)
            {
                var ponto = pontos[pointIndex];
                Perfil.AddPontoEmPm(Poço.Trajetória, ponto.Pm, ponto.Valor, TipoProfundidade.PM, OrigemPonto.Montado);
            }

        }
        /// <returns>Retorna o novo perfil.</returns>
        public PerfilBase ComporPerfil()
        {
            var validator = new ComporPerfilValidator();
            var result = validator.Validate(this);

            if (result.IsValid == false)
            {
                throw new Exception(string.Join(',', result.Errors));
            }

            return Perfil;
        }

        private bool CriarNovoPontoNoPoçoEmPv(Poço poço, Profundidade profundidade, double valor, out Ponto ponto)
        {
            if (!poço.Trajetória.TryGetMDFromTVD(profundidade.Valor, out var pm))
            {
                ponto = null;
                return false;
            }


            var pmProf = new Profundidade(pm);
            ponto = new Ponto(pmProf, pmProf, valor, TipoProfundidade.PM, OrigemPonto.Montado, poço.Trajetória, poço.ObterLitologiaPadrão());

            ponto.AtualizarPV();
            ponto.TrocarProfundidadeReferenciaParaPV();

            return true;
        }

    }
}
