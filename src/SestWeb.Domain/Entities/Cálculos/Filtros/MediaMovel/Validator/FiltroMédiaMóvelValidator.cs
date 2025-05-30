using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.Validator;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.Validator
{
    public class FiltroMédiaMóvelValidator : CálculoValidator<FiltroMédiaMóvel>, IFiltroMédiaMóvelValidator
    {
        public FiltroMédiaMóvelValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator) : base(perfisEntradaValidator, perfisSaídaValidator)
        {
            // Validar aqui particularidades do filtro
            RuleFor(filtro => filtro.NúmeroPontos).GreaterThanOrEqualTo(3);
            RuleFor(filtro => filtro.NúmeroPontos).Custom((n, context) =>
            {
                if (n % 2 == 0)
                {
                    context.AddFailure($"Permitido somente valores ímpares");
                }

            });

            RuleFor(filtro => filtro.LimiteInferior).GreaterThan(0).When(filtro => filtro.LimiteInferior.HasValue);
            RuleFor(filtro => filtro.LimiteSuperior).GreaterThan(0).When(filtro => filtro.LimiteSuperior.HasValue);

        }
    }
}
