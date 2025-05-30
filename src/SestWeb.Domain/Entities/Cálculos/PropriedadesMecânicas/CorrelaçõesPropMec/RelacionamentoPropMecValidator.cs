using FluentValidation;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public sealed class RelacionamentoPropMecValidator : AbstractValidator<RelacionamentoUcsCoesaAngatPorGrupoLitológico>, IRelacionamentoPropMecValidator
    {
        public RelacionamentoPropMecValidator(IAutorValidator autorValidator)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(rel => rel.Nome).NotNull().When(relacionamento => relacionamento != null).WithMessage("Nome do relacionamento deve ser informado!");

            RuleFor(rel => rel.Autor).SetValidator(autorValidator as AbstractValidator<IAutor>).When(relacionamento => relacionamento != null);
        }

        protected override bool PreValidate(ValidationContext<RelacionamentoUcsCoesaAngatPorGrupoLitológico> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(RelacionamentoUcsCoesaAngatPorGrupoLitológico).Name, "Relacionamento não pode ser null!"));
                return false;
            }
            return true;
        }
    }
}
