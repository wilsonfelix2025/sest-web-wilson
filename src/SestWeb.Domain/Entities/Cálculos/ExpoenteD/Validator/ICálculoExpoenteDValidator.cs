using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.Validator
{
    public interface ICálculoExpoenteDValidator
    {
        ValidationResult Validate(CálculoExpoenteD cálculo);
    }
}
