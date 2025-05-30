using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Base.EmCriação
{
    public interface ICálculoEmCriaçãoValidator<T>
    {
        ValidationResult Validate(T cálculoEmCriação);
    }
}
