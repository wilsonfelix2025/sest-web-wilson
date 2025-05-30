using FluentValidation;
using SestWeb.Api.Helpers.Validations;

namespace SestWeb.Api.UseCases.Usuário.EsqueceuSenha.Validações
{
    public class EsqueceuSenhaRequestValidation : AbstractValidator<EsqueceuSenhaRequest>
    {
        public EsqueceuSenhaRequestValidation()
        {
            RuleFor(m => m.Email).NotEmpty().EmailAddress().ApenasEmailPetrobrasEPuc();
        }
    }
}
