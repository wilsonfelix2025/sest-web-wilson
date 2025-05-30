using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.EmCriação
{
    public interface ICálculoPropriedadesMecânicasEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoPropriedadesMecânicasEmCriação cálculoPropriedadesMecânicasEmCriação);
    }
}
