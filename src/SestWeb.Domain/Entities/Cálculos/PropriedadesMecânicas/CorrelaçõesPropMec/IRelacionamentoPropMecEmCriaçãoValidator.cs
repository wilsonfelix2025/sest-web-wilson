using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public interface IRelacionamentoPropMecEmCriaçãoValidator
    {
        ValidationResult Validate(RelacionamentoPropMecEmCriação relacionamentoPropMecEmCriação);
    }
}
