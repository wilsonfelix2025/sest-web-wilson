using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public interface IRelacionamentoPropMecFactory
    {
        ValidationResult CreateRelacionamentoPropMec(string grupoLitológico, string origem, string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr, out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoUcsCoesaAngatPorGrupoLitológico);

        ValidationResult UpdateRelacionamentoPropMec(GrupoLitologico grupoLitológico, string origem, string nomeAutor,
            string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr,
            out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoUcsCoesaAngatPorGrupoLitológico);
    }
}
