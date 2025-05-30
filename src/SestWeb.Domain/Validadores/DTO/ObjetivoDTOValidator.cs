using System.Globalization;
using FluentValidation;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Domain.Validadores.DTO
{
    public class ObjetivoDTOValidator : AbstractValidator<ObjetivoDTO>
    {
        public ObjetivoDTOValidator()
        {
            RuleFor(s => s.Pm)
                .NotEmpty()
                .Custom((x, context) =>
                {
                    if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) || value < 0))
                    {
                        context.AddFailure($"{x} não é um número válido para Pm de Objetivo ou é menor que 0");
                    }
                });

            RuleFor(s => s.TipoObjetivo)
                .NotEmpty()
                .IsEnumName(typeof(TipoObjetivo), caseSensitive: false);
        }
    }
}
