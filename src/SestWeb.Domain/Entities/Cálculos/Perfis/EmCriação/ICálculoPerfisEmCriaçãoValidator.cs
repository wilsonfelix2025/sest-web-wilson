using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Perfis.EmCriação
{
    public interface ICálculoPerfisEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoPerfisEmCriação cálculoPerfisEmCriação);
    }
}
