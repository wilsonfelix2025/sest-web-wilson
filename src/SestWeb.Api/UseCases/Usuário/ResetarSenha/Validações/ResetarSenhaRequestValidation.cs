using FluentValidation;
using SestWeb.Api.Helpers.Validations;

namespace SestWeb.Api.UseCases.Usuário.ResetarSenha.Validações
{
    public class ResetarSenhaRequestValidation : AbstractValidator<ResetarSenhaRequest>
    {
        private const int PasswordMaximumLength = 10;
        private const int PasswordMinimumLength = 6;
        public ResetarSenhaRequestValidation()
        {
            RuleFor(m => m.Email).NotEmpty().EmailAddress().ApenasEmailPetrobrasEPuc();
            RuleFor(m => m.Senha).NotEmpty()
                .MaximumLength(PasswordMaximumLength).WithMessage($"Senha deve ter, no máximo, {PasswordMaximumLength} caracteres alfanuméricos.")
                .MinimumLength(PasswordMinimumLength).WithMessage($"Senha deve ter, pelo menos, {PasswordMinimumLength} caracteres alfanuméricos.")
                .Matches("[a-z]").WithMessage("Senha deve conter, pelo menos, uma letra minúscula.")
                .Matches("[A-Z]").WithMessage("Senha deve conter, pelo menos, uma letra maiúscula.")
                .Matches("[0-9]").WithMessage("Senha deve conter, pelo menos, um número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Senha deve conter, pelo menos um, caracter especial.");
            RuleFor(x => x.Codigo).NotEmpty();
        }
    }
}
