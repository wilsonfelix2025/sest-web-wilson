using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Validator
{
    public interface ICálculoPropriedadesMecânicasValidator
    {
        ValidationResult Validate(CálculoPropriedadesMecânicas cálculo);
    }
}
