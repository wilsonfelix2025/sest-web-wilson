using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Tensões.Validator
{
    public interface ICálculoTensõesValidator
    {
        ValidationResult Validate(CálculoTensões cálculo);
    }
}
