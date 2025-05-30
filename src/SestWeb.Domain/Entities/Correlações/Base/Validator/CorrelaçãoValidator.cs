using FluentValidation;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.Validator;

namespace SestWeb.Domain.Entities.Correlações.Base.Validator
{
    public sealed class CorrelaçãoValidator : AbstractValidator<Correlação>, ICorrelaçãoValidator
    {
        private readonly IAutorValidator _autorValidator;
        private readonly IExpressionValidator _expressionValidator;

        public CorrelaçãoValidator(IAutorValidator autorValidator, IExpressionValidator expressionValidator)
        {
            _autorValidator = autorValidator;
            _expressionValidator = expressionValidator;
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(corr => corr.Nome).NotNull().When(correlação => correlação != null).WithMessage("Nome da Correlação deve ser informado!");

            RuleFor(corr => corr.Autor).SetValidator(_autorValidator as AbstractValidator<IAutor>).When(correlação => correlação != null);

            RuleFor(corr => corr.Descrição).NotNull().When(correlação => correlação != null).WithMessage("Descrição da Correlação deve ser informada!");

            RuleFor(corr => corr.Expressão).SetValidator(_expressionValidator as AbstractValidator<IExpressão>).When(correlação => correlação != null);
        }

        protected override bool PreValidate(ValidationContext<Correlação> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(Correlação).Name, "Correlação não pode ser null!"));
                return false;
            }
            return true;
        }
    }
}
