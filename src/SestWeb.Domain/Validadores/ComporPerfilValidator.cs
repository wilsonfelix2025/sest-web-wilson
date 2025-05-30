using FluentValidation;
using SestWeb.Domain.ComposiçãoPerfil;

namespace SestWeb.Domain.Validadores
{
    public class ComporPerfilValidator : AbstractValidator<CompositorDePerfil>
    {
        public ComporPerfilValidator()
        {
            RuleFor(montador => montador.Poço).NotEmpty().WithMessage("Poço não encontrado");

            RuleFor(montador => montador.Poço.Trajetória.Pontos).NotEmpty();

            RuleFor(montador => montador.Perfil).NotEmpty();
        }
    }
}
