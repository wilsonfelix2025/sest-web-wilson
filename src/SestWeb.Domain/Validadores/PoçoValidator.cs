using System.Linq;
using FluentValidation;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Domain.Validadores
{
    public class PoçoValidator : AbstractValidator<Poço>
    {
        public PoçoValidator()
        {
            RuleFor(p => p.DadosGerais.Identificação.NomePoço).NotEmpty();
            RuleFor(p => p.DadosGerais.Identificação.ComplexidadePoço).GreaterThanOrEqualTo(0);
            RuleFor(p => p.DadosGerais.Identificação.VidaÚtilPrevista).GreaterThanOrEqualTo(0);

            #region Geometria

            RuleFor(p => p.DadosGerais.Geometria.MesaRotativa).GreaterThanOrEqualTo(0);

            When(p => p.DadosGerais.Geometria.OffShore.LaminaDagua.Equals(0),
                () =>
                {
                    RuleFor(p => p.DadosGerais.Geometria.OnShore.LençolFreático).GreaterThanOrEqualTo(0);

                    RuleFor(p => p.DadosGerais.Geometria.OnShore.Elevação).GreaterThanOrEqualTo(0);

                    RuleFor(p => p.DadosGerais.Geometria.OnShore.AlturaDeAntePoço).GreaterThanOrEqualTo(0);
                });

            When(p => p.DadosGerais.Geometria.OnShore.Elevação.Equals(0),
                () => { RuleFor(p => p.DadosGerais.Geometria.OffShore.LaminaDagua).GreaterThanOrEqualTo(0); });

            RuleFor(p => p.DadosGerais.Geometria.Coordenadas.UtMx).GreaterThanOrEqualTo(0);

            RuleFor(p => p.DadosGerais.Geometria.Coordenadas.UtMy).GreaterThanOrEqualTo(0);

            #endregion

            #region Area

            RuleFor(p => p.DadosGerais.Area.DensidadeSuperficie).GreaterThan(0);

            RuleFor(p => p.DadosGerais.Area.DensidadeAguaMar).GreaterThan(0);

            RuleFor(p => p.DadosGerais.Area.SonicoSuperficie).GreaterThan(0);
            #endregion

            RuleForEach(p => p.Litologias).SetValidator(new LitologiaValidator());
            RuleFor(x => x.Litologias).Custom((litos, context) =>
            {
                var temLitologiasComMesmoTipo = litos.GroupBy(x => x.Classificação).Any(g => g.Count() > 1);

                if (temLitologiasComMesmoTipo)
                {
                    context.AddFailure($"Há litologias com o mesmo tipo repetido");
                }
            });

            RuleForEach(p => p.Sapatas).SetValidator(new SapataValidator());
            RuleForEach(p => p.Objetivos).SetValidator(new ObjetivoValidator());
            RuleFor(p => p.Trajetória).SetValidator(new TrajetoriaValidator());
            RuleFor(x => x.Perfis).Custom((perfis, context) =>
            {
                var temPerfilComNomeDuplicado = perfis.GroupBy(x => x.Nome).Any(g => g.Count() > 1);

                if (temPerfilComNomeDuplicado)
                {
                    //context.AddFailure($"Há nomes repetidos para os perfis");
                }
            });


            RuleForEach(p => p.Perfis).Custom((perfis, context) =>
            {
                var poço = context.ParentContext.InstanceToValidate as Poço;
                var soma = poço.ObterBaseDeSedimentos();

                if (perfis.PrimeiroPonto != null && perfis.PmMínimo.Valor < soma)
                {
                    context.AddFailure($"Primeiro ponto do perfil " + perfis.Nome + " está numa profundidade inválida");
                }
            });

            RuleForEach(p => p.Perfis).SetValidator(new PerfilValidator());

        }
    }
}
