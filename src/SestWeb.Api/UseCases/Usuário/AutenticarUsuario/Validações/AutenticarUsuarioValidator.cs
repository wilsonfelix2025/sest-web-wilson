using FluentValidation;

namespace SestWeb.Api.UseCases.Usuário.AutenticarUsuario.Validações
{
    public class AutenticarUsuarioValidator :  AbstractValidator<AutenticarUsuarioRequest>
    {
        public AutenticarUsuarioValidator()
        {
            RuleFor(m => m.Email).NotEmpty();
            RuleFor(m => m.Senha).NotEmpty();
        }
    }
}
