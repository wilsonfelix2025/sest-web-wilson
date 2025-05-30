using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.EmCriação
{
    public interface ICálculoPressãoPorosEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoPressãoPorosEmCriação cálculoPerfisEmCriação);
    }
}
