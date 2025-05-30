using FluentValidation;
using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Domain.Validadores
{
    public class ObjetivoValidator : AbstractValidator<Objetivo>
    {
        public ObjetivoValidator()
        {
            RuleFor(s => s.Pm)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(s => s.TipoObjetivo)
                .IsInEnum();
        }
    }
}
