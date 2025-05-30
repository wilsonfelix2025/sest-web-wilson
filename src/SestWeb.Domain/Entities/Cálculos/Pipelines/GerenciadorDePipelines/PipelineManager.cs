using System.Collections.Generic;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines
{
    public class PipelineManager : IPipelineManager
    {
        private readonly List<ICálculo> _cálculosDoPoço;
        private readonly List<IFiltro> _filtrosDoPoço;
        private readonly CalculusDependenceManager _calculusDependenceManager;
        private readonly PerfilDependenceManager _perfilDependenceManager;
        private readonly TrendDependenceManager _trendDependenceManager;
        private readonly LitologyDependenceManager _litologyDependenceManager;
        private readonly IOrdenadorCálculosDependentes _ordenadorCálculos;

        public List<ICálculo> CálculosDoPoço => _cálculosDoPoço;
        public List<IFiltro> FiltrosDoPoço => _filtrosDoPoço;
        public IOrdenadorCálculosDependentes OrdenadorCálculos => _ordenadorCálculos;

        public PipelineManager(List<ICálculo> cálculosDoPoço, List<IFiltro> filtrosDoPoço)
        {
            _cálculosDoPoço = cálculosDoPoço;
            _filtrosDoPoço = filtrosDoPoço;
            _calculusDependenceManager = new CalculusDependenceManager(this);
            _perfilDependenceManager = new PerfilDependenceManager(this);
            _trendDependenceManager = new TrendDependenceManager(this);
            _litologyDependenceManager = new LitologyDependenceManager(this);
            _ordenadorCálculos = new OrdenadorCálculosDependentes();
        }

        #region Dependências do Perfil

        public List<ICálculo> GetDependentCalculus(PerfilBase perfil)
        {
            return _perfilDependenceManager.GetDependentCalculus(perfil.Id.ToString());
        }

        public List<IFiltro> GetDependentFilters(PerfilBase perfil)
        {
            return _perfilDependenceManager.GetDependentFilters(perfil);
        }

        public List<ICálculo> ObterCálculosDependentesDosPerfisFiltradosDependentes(string idPerfil)
        {
            return _perfilDependenceManager.ObterCálculosDependentesDosPerfisFiltradosDependentes(idPerfil);
        }

        public void InvalidateDependentCalculus(PerfilBase perfil, List<ICálculo> cálculosDependentes)
        {
            _perfilDependenceManager.InvalidateDependentCalculus(perfil,cálculosDependentes);
        }

        #endregion

        #region Dependências do Cálculo

        public List<ICálculo> GetDependentCalculus(ICálculo cálculo)
        {
            return _calculusDependenceManager.GetDependentCalculus(cálculo);
        }

        public List<IFiltro> GetDependentFilters(ICálculo cálculo)
        {
            return  _calculusDependenceManager.GetDependentFilters(cálculo);
        }

        public void InvalidateDependentCalculus(List<ICálculo> cálculosDependentes)
        {
            _calculusDependenceManager.InvalidateDependentCalculus(cálculosDependentes);
        }

        public bool VerificarDepleçãoContémPerfil(ICálculo cálculo, string perfilId)
        {
            return _calculusDependenceManager.VerificarDepleçãoContémPerfil(cálculo, perfilId);
        }

        #endregion

        #region Dependências do Trend

        public List<ICálculo> GetDependentCalculus(Trend.Trend trend, List<PerfilBase> perfisDoPoço)
        {
            return _trendDependenceManager.GetDependentCalculus(trend, perfisDoPoço);
        }

        #endregion

        #region Dependências da Litologia

        public List<ICálculo> ObterCálculosDependentesDaLitologia(ILitologia litologia)
        {
            return _litologyDependenceManager.GetDependentCalculus(litologia);
        }

        public List<IFiltro> ObterFiltrosDependentesDaLitologia(ILitologia litologia)
        {
            return _litologyDependenceManager.GetDependentFilters(litologia);
        }

        #endregion

        #region Dependências do Trend

        public List<IFiltro> ObterFiltrosDependentesDoTrend(Trend.Trend trend, List<PerfilBase> perfisDoPoço)
        {
            return _trendDependenceManager.GetDependentFilters(trend, perfisDoPoço);
        }

        public void InvalidateDependentCalculus(Trend.Trend trend, List<PerfilBase> perfisDoPoço)
        {
            _trendDependenceManager.InvalidateDependentCalculus(trend, perfisDoPoço);
        }

        #endregion
    }
}
