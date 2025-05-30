using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.EmCriação
{
    public interface ICálculoGradienteSobrecargaEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoGradienteSobrecargaEmCriação cálculoPerfisEmCriação);
    }
}
