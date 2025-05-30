using System.Globalization;
using FluentValidation;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Validadores.DTO
{
    public class SapataDTOValidator : AbstractValidator<SapataDTO>
    {
        public SapataDTOValidator()
        {
            RuleFor(s => s.Pm)
                .NotEmpty()
                .Custom((x, context) =>
                {
                    if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) || value < 0))
                    {
                        context.AddFailure($"{x} não é um número válido para Pm de Sapata ou é menor que 0");
                    }
                });

            RuleFor(s => s.Diâmetro)
                .NotEmpty()
                .Custom((x, context) =>
                {
                     
                });
        }
    }
}
