using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public interface IRelacionamentoPropMecValidator
    {
        ValidationResult Validate(
            RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoUcsCoesaAngatPorGrupoLitológico);
    }
}
