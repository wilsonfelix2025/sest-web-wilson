using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.EmCriação
{
    public interface ICálculoGradientesEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoGradientesEmCriação cálculoPerfisEmCriação);
    }
}
