using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Validator
{
    public interface ICálculoGradientesValidator
    {
        ValidationResult Validate(CálculoGradientes cálculo);
    }
}
