using FluentValidation;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Validadores.DTO
{
    public class EstratigrafiaDTOValidator : AbstractValidator<EstratigrafiaDTO>
    {
        public EstratigrafiaDTOValidator()
        {
            RuleForEach(e => e.Itens)
                .ChildRules(i => i.RuleFor(x => x.Value)
                    .Custom((x, context) =>
                {
                    foreach (var y in x)
                    {
                        if ((!(double.TryParse(y.Pm, out var value)) || value <= 0))
                        {
                            context.AddFailure($"{y.Pm} não é um número válido ou é menor que 0");
                        }
                    }
                }));
        }
    }
}
