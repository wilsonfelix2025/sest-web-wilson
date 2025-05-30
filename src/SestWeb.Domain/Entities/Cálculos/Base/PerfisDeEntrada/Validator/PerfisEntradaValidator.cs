using FluentValidation;
using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Validator
{
    public class PerfisEntradaValidator : AbstractValidator<IPerfisEntrada>, IPerfisEntradaValidator
    {
        public PerfisEntradaValidator()
        {
            RuleFor(perfisDeEntrada => perfisDeEntrada.Perfis).NotNull();

            RuleForEach(perfisDeEntrada => perfisDeEntrada.Perfis).NotNull().WithMessage("Perfil, index: {CollectionIndex}, está null.");

            //RuleForEach(perfisDeEntrada => perfisDeEntrada.Perfis)
            //    .Must(IsValid).WithMessage("Perfil, index: {CollectionIndex}, não é um perfil válido."); 
        }

        //private bool IsValid(Perfil tipo)
        //{
        //    TODO(RCM): deve validar se o tipo do perfil está na coleção de tipos de perfis de entrada da correlação.
        //}

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
