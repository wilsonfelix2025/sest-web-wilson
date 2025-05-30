using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Perfis.Validator
{
    public interface ICálculoPerfisValidator
    {
        ValidationResult Validate(CálculoPerfis cálculo);
    }
}
