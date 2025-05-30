using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Base.Validator
{
    public interface ICálculoValidator<T>
    {
        ValidationResult Validate(T cálculo);
    }
}
