using FluentValidation;

namespace SestWeb.Api.UseCases.Usuário.TrocarSenha.Validações
{
    public class TrocarSenhaRequestValidation : AbstractValidator<TrocarSenhaRequest>
    {
        private const int PasswordMaximumLength = 10;
        private const int PasswordMinimumLength = 6;

        public TrocarSenhaRequestValidation()
        {
            RuleFor(m => m.SenhaAntiga).NotEmpty()
                .MaximumLength(PasswordMaximumLength).WithMessage($"Senha deve ter, no máximo, {PasswordMaximumLength} caracteres alfanuméricos.")
                .MinimumLength(PasswordMinimumLength).WithMessage($"Senha deve ter, pelo menos, {PasswordMinimumLength} caracteres alfanuméricos.")
                .Matches("[a-z]").WithMessage("Senha deve conter, pelo menos, uma letra minúscula.")
                .Matches("[A-Z]").WithMessage("Senha deve conter, pelo menos, uma letra maiúscula.")
                .Matches("[0-9]").WithMessage("Senha deve conter, pelo menos, um número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Senha deve conter, pelo menos um, caracter especial.");

            RuleFor(m => m.NovaSenha).NotEmpty()
                .MaximumLength(PasswordMaximumLength).WithMessage($"Senha deve ter, no máximo, {PasswordMaximumLength} caracteres alfanuméricos.")
                .MinimumLength(PasswordMinimumLength).WithMessage($"Senha deve ter, pelo menos, {PasswordMinimumLength} caracteres alfanuméricos.")
                .Matches("[a-z]").WithMessage("Senha deve conter, pelo menos, uma letra minúscula.")
                .Matches("[A-Z]").WithMessage("Senha deve conter, pelo menos, uma letra maiúscula.")
                .Matches("[0-9]").WithMessage("Senha deve conter, pelo menos, um número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Senha deve conter, pelo menos um, caracter especial.");
        }
    }
}
