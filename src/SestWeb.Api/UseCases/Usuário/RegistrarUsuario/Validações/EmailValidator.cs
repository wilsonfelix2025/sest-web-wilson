using FluentValidation.Validators;

namespace SestWeb.Api.UseCases.Usuário.RegistrarUsuario.Validações
{
    public class EmailValidator : PropertyValidator
    {
        public EmailValidator() : base("E-mail: '{PropertyValue}' não é válido. Apenas e-mails com domínio @petrobras.com.br ou @puc-rio.br são permitidos.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var email = context.PropertyValue as string;

            if (email == null) return false;

            if (email.Contains("@petrobras.com.br") || email.Contains("@puc-rio.br"))
            {
                return true;
            }

            return false;
        }
    }
}
