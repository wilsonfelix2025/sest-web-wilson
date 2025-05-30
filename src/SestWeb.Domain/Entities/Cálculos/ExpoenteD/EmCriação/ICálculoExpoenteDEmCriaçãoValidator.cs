using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.EmCriação
{
    public interface ICálculoExpoenteDEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoExpoenteDEmCriação cálculoPerfisEmCriação);
    }
}
