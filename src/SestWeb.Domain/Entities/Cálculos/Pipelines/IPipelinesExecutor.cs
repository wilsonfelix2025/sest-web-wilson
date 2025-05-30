using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines
{
    public interface IPipelinesExecutor
    {
        List<ICálculo> GetDependentCalculus(PerfilBase perfil);
        List<ICálculo> GetDependentCalculus(ILitologia litology);
        List<ICálculo> GetDependentCalculus(Trend.Trend trend, List<PerfilBase> perfisDoPoço);
        List<ICálculo> GetDependentCalculus(ICálculo executedCalculus);
        IList<PerfilBase> Recalculate(List<ICálculo> executedCalculus);


    }
}