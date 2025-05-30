using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga.Validator
{
    public interface ICálculoSobrecargaValidator
    {
        ValidationResult Validate(CálculoSobrecarga cálculo);
    }
}
