using System;
using FluentValidation;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.EditingRelacionamentoPropMec
{
    public class RelacionamentoPropMecEmEdiçãoValidator : AbstractValidator<RelacionamentoPropMecEmEdição>, IRelacionamentoPropMecEmEdiçãoValidator
    {
        public RelacionamentoPropMecEmEdiçãoValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(rel => rel.Origem)
                .Must(origem => OrigemDeveSerVálida(origem))
                .When(relacionamento => relacionamento != null)
                .WithMessage(
                    $"Origem do relacionamento deve ser uma das seguintes opções: {"\n"}{string.Join("\n", Enum.GetNames(typeof(Origem)))}!");

            RuleFor(rel => rel.Origem)
                .Must(origem => OrigemDeveSerUsuário(origem))
                .When(correlação => correlação != null)
                .WithMessage($"Somente relacionamentos do usuário são editáveis!");
        }

        protected override bool PreValidate(ValidationContext<RelacionamentoPropMecEmEdição> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(RelacionamentoPropMecEmEdição).Name,
                    "Relacionamento de Correlações de Propriedades Mecânicas não pode ser null!"));
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
