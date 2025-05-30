using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.TensãoVertical.EmCriação
{
    interface ICálculoTensãoVerticalEmCriaçãoValidator
    {
        ValidationResult Validate(CálculoTensãoVerticalEmCriação cálculoTensõesEmCriação);
    }
}
