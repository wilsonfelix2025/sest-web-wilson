using FluentValidation;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;

namespace SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação.Validator
{
    public class PerfisEntradaValidator : AbstractValidator<IPerfisEntrada>, IPerfisEntradaValidator
    {
        public PerfisEntradaValidator()
        {
            RuleFor(perfisDeEntrada => perfisDeEntrada.Tipos).NotNull();// pode ser empty

            RuleForEach(perfisDeEntrada => perfisDeEntrada.Tipos).NotNull().WithMessage("Perfil, index: {CollectionIndex}, está null.");

            RuleForEach(perfisDeEntrada => perfisDeEntrada.Tipos)
                .Must(IsValid).WithMessage("Perfil, index: {CollectionIndex}, não é um perfil válido."); ;
        }

        private bool IsValid(string tipo)
        {
            return TiposVálidos.Perfis.Find(p => p.Equals(tipo)) != null;
        }

        protected override bool PreValidate(ValidationContext<IPerfisEntrada> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(IPerfisEntrada).Name, $"PerfisEntrada não pode ser null."));
                return false;
            }
            return true;
        }
    }
}
