using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Tensões.EmCriação
{
    public interface ICálculoTensõesEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoTensõesEmCriação cálculoTensõesEmCriação);
    }
}
