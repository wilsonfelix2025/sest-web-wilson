using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public interface ILoaderRelacionamentosPropMec
    {
        List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> Load(List<Correlação> correlações);
        bool IsCorrsLoaded();
        List<RelacionamentoUcsCoesaAngatPorGrupoLitológico> GetRelacionamentos();
    }
}
