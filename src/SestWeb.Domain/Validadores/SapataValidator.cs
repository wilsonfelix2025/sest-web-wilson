using FluentValidation;
using SestWeb.Domain.Entities.Poço.Sapatas;

namespace SestWeb.Domain.Validadores
{
    public class SapataValidator : AbstractValidator<Sapata>
    {
        public SapataValidator()
        {
            RuleFor(s => s.Pm)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(s => s.Diâmetro)
                .NotEmpty();
        }
    }
}
