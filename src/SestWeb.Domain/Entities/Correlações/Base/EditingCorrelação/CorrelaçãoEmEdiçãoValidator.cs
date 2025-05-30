using FluentValidation;
using System;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Domain.Entities.Correlações.Base.EditingCorrelação
{
    public class CorrelaçãoEmEdiçãoValidator : AbstractValidator<CorrelaçãoEmEdição>, ICorrelaçãoEmEdiçãoValidator
    {
        public CorrelaçãoEmEdiçãoValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(corr => corr.Origem)
                .Must(origem => OrigemDeveSerVálida(origem))
                .When(correlação => correlação != null)
                .WithMessage(
                    $"Origem da correlação deve ser uma das seguintes opções: {"\n"}{string.Join("\n", Enum.GetNames(typeof(Origem)))}!");

            RuleFor(corr => corr.Origem)
                .Must(origem => OrigemDeveSerUsuário(origem))
                .When(correlação => correlação != null)
                .WithMessage($"Somente correlações do usuário são editáveis!");
        }

        protected override bool PreValidate(ValidationContext<CorrelaçãoEmEdição> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(CorrelaçãoEmEdição).Name,
                    "Correlação não pode ser null!"));
                return false;
            }

            return true;
        }

        private bool OrigemDeveSerVálida(string origem)
        {
            return Enum.TryParse(origem, out Origem value);
        }

        private bool OrigemDeveSerUsuário(string origem)
        {
            return Enum.TryParse(origem, out Origem value) && (value.Equals(Origem.Usuário) || value.Equals(Origem.Poço));
        }
    }
}
