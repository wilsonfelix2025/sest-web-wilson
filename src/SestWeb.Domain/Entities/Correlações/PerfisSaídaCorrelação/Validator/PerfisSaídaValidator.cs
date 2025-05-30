using FluentValidation;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;

namespace SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação.Validator
{
    public sealed class PerfisSaídaValidator : AbstractValidator<IPerfisSaída>, IPerfisSaídaValidator
    {
        public PerfisSaídaValidator()
        {
            RuleFor(perfisDeSaída => perfisDeSaída.Tipos).NotNull().NotEmpty().WithMessage("Perfil de Saída não identificado na expressão.");

            RuleForEach(perfisDeSaída => perfisDeSaída.Tipos).NotNull().WithMessage("Perfil, index: {CollectionIndex}, está null.");

            RuleForEach(perfisDeSaída => perfisDeSaída.Tipos)
                .Must(IsValid).WithMessage("Perfil, index: {CollectionIndex}, não é um perfil válido.");
        }

        private bool IsValid(string tipo)
        {
            return TiposVálidos.Perfis.Find(p => p.Equals(tipo)) != null;
        }

        protected override bool PreValidate(ValidationContext<IPerfisSaída> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(IPerfisSaída).Name, $"PerfisSaída não pode ser null."));
                return false;
            }
            return true;
        }
    }
}
