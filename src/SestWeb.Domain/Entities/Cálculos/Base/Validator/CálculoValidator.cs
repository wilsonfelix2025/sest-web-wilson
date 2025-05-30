 using FluentValidation;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator;

namespace SestWeb.Domain.Entities.Cálculos.Base.Validator
{
    public class CálculoValidator<T> : AbstractValidator<T>, ICálculoValidator<T> where T : Cálculo 
    {
        protected CálculoValidator(IPerfisEntradaValidator perfisEntradaValidator, IPerfisSaídaValidator perfisSaídaValidator)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(calc => calc.Nome).NotNull().When(cálculo => cálculo != null).WithMessage("Nome do cálculo deve ser informado!");

            RuleFor(calc => calc.PerfisEntrada).SetValidator(perfisEntradaValidator as AbstractValidator<IPerfisEntrada>).When(cálculo => cálculo != null);

            RuleFor(calc => calc.PerfisSaída).SetValidator(perfisSaídaValidator as AbstractValidator<IPerfisSaída>).When(cálculo => cálculo != null);
        }

        protected override bool PreValidate(ValidationContext<T> validationContext, ValidationResult result)
        {
            if (validationContext.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(T).Name, "Cálculo não pode ser null!"));
                return false;
            }
            return true;
        }
    }
}
