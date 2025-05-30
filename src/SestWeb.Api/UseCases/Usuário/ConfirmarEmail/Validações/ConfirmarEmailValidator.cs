using FluentValidation;

namespace SestWeb.Api.UseCases.Usuário.ConfirmarEmail.Validações
{
    public class ConfirmarEmailValidator: AbstractValidator<ConfirmarEmailRequest>
    {
        public ConfirmarEmailValidator()
        {
            RuleFor(x => x.IdUsuario).NotEmpty();
            RuleFor(x => x.Codigo).NotEmpty();
        }
    }
}
