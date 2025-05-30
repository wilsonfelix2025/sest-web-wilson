using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.EditingRelacionamentoPropMec
{
    public interface IRelacionamentoPropMecEmEdiçãoValidator
    {
        ValidationResult Validate(RelacionamentoPropMecEmEdição relacionamentoEmEdição);
    }
}
