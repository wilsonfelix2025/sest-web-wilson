using FluentValidation;
using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Correlações.AutorCorrelação
{
    public class AutorValidator : AbstractValidator<IAutor>, IAutorValidator
    {
        public AutorValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(autor => autor.Nome).NotNull().NotEmpty();

            RuleFor(autor => autor.Chave).NotNull().NotEmpty();

            RuleFor(autor => autor.DataCriação).NotNull().NotEmpty();
        }

        protected override bool PreValidate(ValidationContext<IAutor> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(IAutor).Name, $"{typeof(IAutor).Name} não pode ser null."));
                return false;
            }
            return true;
        }
    }
}
