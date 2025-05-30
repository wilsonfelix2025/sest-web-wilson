using FluentValidation;
using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Validator
{
    public sealed class PerfisSaídaValidator : AbstractValidator<IPerfisSaída>, IPerfisSaídaValidator
    {
        public PerfisSaídaValidator()
        {
            RuleFor(perfisDeSaída => perfisDeSaída.Perfis).NotNull().NotEmpty().WithMessage("Perfil de Saída não identificado na expressão.");

            RuleForEach(perfisDeSaída => perfisDeSaída.Perfis).NotNull().WithMessage("Perfil, index: {CollectionIndex}, está null.");

            //RuleForEach(perfisDeSaída => perfisDeSaída.Tipos)
            //    .Must(IsValid).WithMessage("Perfil, index: {CollectionIndex}, não é um perfil válido.");
        }

        //private bool IsValid(Perfil tipo)
        //{
        //    TODO(RCM): deve validar se o tipo do perfil está na coleção de tipos de perfis de saída da correlação.
        //}

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
