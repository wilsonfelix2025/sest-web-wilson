using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Validator
{
    public interface ICálculoPressãoPorosValidator
    {
        ValidationResult Validate(CálculoPressãoPoros cálculo);
    }
}
