using System.Collections.Generic;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines
{
    public interface IPipelineManager
    {
        List<ICálculo> CálculosDoPoço { get; }
        List<IFiltro> FiltrosDoPoço { get; }
        IOrdenadorCálculosDependentes OrdenadorCálculos { get; }

        List<ICálculo> GetDependentCalculus(PerfilBase perfil);

        void InvalidateDependentCalculus(PerfilBase perfil, List<ICálculo> cálculosDependentes);

        List<ICálculo> GetDependentCalculus(ICálculo cálculo);

        List<IFiltro> GetDependentFilters(ICálculo cálculo);

        List<ICálculo> ObterCálculosDependentesDaLitologia(ILitologia litologia);

        List<ICálculo> ObterCálculosDependentesDosPerfisFiltradosDependentes(string idPerfil);

        List<IFiltro> GetDependentFilters(PerfilBase perfil);

        List<IFiltro> ObterFiltrosDependentesDaLitologia(ILitologia litologia);

        List<IFiltro> ObterFiltrosDependentesDoTrend(Trend.Trend trend, List<PerfilBase> perfisDoPoço);

        List<ICálculo> GetDependentCalculus(Trend.Trend trend, List<PerfilBase> perfisDoPoço);

        void InvalidateDependentCalculus(Trend.Trend trend, List<PerfilBase> perfisDoPoço);

        void InvalidateDependentCalculus(List<ICálculo> cálculosDependentes);

        bool VerificarDepleçãoContémPerfil(ICálculo cálculo, string id);
    }
}