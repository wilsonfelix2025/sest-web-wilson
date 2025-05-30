using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga.EmCriação
{
    public interface ICálculoSobrecargaEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoSobrecargaEmCriação cálculoPerfisEmCriação);
    }
}
