using FluentValidation;
using SestWeb.Api.UseCases.Usuário.RegistrarUsuario.Validações;

namespace SestWeb.Api.Helpers.Validations
{
    public static class CustomValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> ApenasEmailPetrobrasEPuc<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new EmailValidator());
        }
    }
}
