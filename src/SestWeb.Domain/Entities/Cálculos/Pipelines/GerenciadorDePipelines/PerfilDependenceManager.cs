using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.Cálculos.Tensões;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines
{
    public class PerfilDependenceManager
    {
        private readonly IPipelineManager _pipelineManager;
        private readonly List<ICálculo> _cálculosDoPoço;
        private readonly List<IFiltro> _filtrosDoPoço;
        private readonly IOrdenadorCálculosDependentes _ordenadorCálculos;

        public PerfilDependenceManager(IPipelineManager pipelineManager)
        {
            _pipelineManager = pipelineManager;
            _cálculosDoPoço = pipelineManager.CálculosDoPoço;
            _filtrosDoPoço = pipelineManager.FiltrosDoPoço;

            _ordenadorCálculos = new OrdenadorCálculosDependentes();
        }

        public List<ICálculo> GetDependentCalculus(string perfil)
        {
            var cálculosDependentes = GetDependentCalculus(perfil, _cálculosDoPoço, _filtrosDoPoço);
            return _ordenadorCálculos.Order(cálculosDependentes);
        }

        private List<ICálculo> GetDependentCalculus(string perfil, List<ICálculo> cálculosDoPoço, List<IFiltro> filtros)
        {
            var cálculosDependentes = new List<ICálculo>();

            if (filtros != null && filtros.Count > 0)
            {
                var calcsDepFiltros = ObterCálculosDependentesDosPerfisFiltradosDependentes(perfil);
                foreach (var dependente in calcsDepFiltros)
                {
                    var nãoIncluído = cálculosDependentes.Find(c => c.Id == dependente.Id) == null;
                    if (nãoIncluído)
                        cálculosDependentes.Add(dependente);
                }
            }

            foreach (var cálculo in cálculosDoPoço)
            {
                var cálculoContémPerfil = cálculo.ContémPerfilEntradaPorId(perfil);
                var depleçãoContémPerfil = _pipelineManager.VerificarDepleçãoContémPerfil(cálculo, perfil);

                if (!cálculoContémPerfil && !depleçãoContémPerfil)
                    continue;

                // adiciona o cálculo dependente 
                var nãoIncluído = cálculosDependentes.Find(c => c.Id == cálculo.Id) == null;
                if (nãoIncluído)
                    cálculosDependentes.Add(cálculo);

                // Adiciona os dependentes do cálculo dependentes
                var dependentes = _pipelineManager.GetDependentCalculus(cálculo);
                foreach (var dependente in dependentes)
                {
                    nãoIncluído = cálculosDependentes.Find(c => c.Id == dependente.Id) == null;

                    if (nãoIncluído)
                        cálculosDependentes.Add(dependente);
                }
            }
            return cálculosDependentes;
        }

        public void InvalidateDependentCalculus(PerfilBase perfil, List<ICálculo> cálculosDependentes)
        {
            foreach (var cálculo in cálculosDependentes)
            {
                if (cálculo.ContémPerfilEntradaPorId(perfil.Id.ToString()))
                {
                    cálculo.PerfisEntrada.Perfis.Remove(perfil);
                }
                if (cálculo is CálculoTensões)
                {
                    var cal = (CálculoTensões)cálculo;
                    cal.InvalidatePerfilDepleção(perfil.Id);
                }

                cálculo.PerfisSaída.Perfis.ForEach(perfilSaída =>
                {
                    if (perfilSaída.Mnemonico != "AZTHmin")
                        perfilSaída.Clear();
                });
            }
        }

        public List<ICálculo> ObterCálculosDependentesDosPerfisFiltradosDependentes(string idPerfil)
        {
            List<ICálculo> cálculosDependentesDosFiltrosAplicados = new List<ICálculo>();

            if (_pipelineManager.FiltrosDoPoço == null || _pipelineManager.FiltrosDoPoço.Count == 0)
                return cálculosDependentesDosFiltrosAplicados;

            var filtrosDependentes = _pipelineManager.FiltrosDoPoço.FindAll(f => f.PerfisEntrada.IdPerfis.First() == idPerfil);

            if (filtrosDependentes.Count == 0)
                return cálculosDependentesDosFiltrosAplicados;

            foreach (var filtro in filtrosDependentes)
            {
                var calcDep = GetDependentCalculus(filtro.PerfisSaída.IdPerfis.First(), _pipelineManager.CálculosDoPoço, _pipelineManager.FiltrosDoPoço);

                foreach (var dependente in calcDep)
                {
                    var nãoIncluído = cálculosDependentesDosFiltrosAplicados.Find(c => c.Id == dependente.Id) == null;

                    if (nãoIncluído)
                        cálculosDependentesDosFiltrosAplicados.Add(dependente);
                }
            }

            return cálculosDependentesDosFiltrosAplicados;
        }

        public List<IFiltro> GetDependentFilters(PerfilBase perfil)
        {
            return GetDependentFilters(perfil, _pipelineManager.FiltrosDoPoço, _pipelineManager.CálculosDoPoço).ToList();
        }

        private List<IFiltro> GetDependentFilters(PerfilBase perfil, List<IFiltro> filtros, List<ICálculo> cálculos)
        {
            List<IFiltro> filtrosDependentes = new List<IFiltro>();

            if (filtros == null || filtros.Count == 0)
            {
                return filtrosDependentes;
            }

            // Filtros dependentes dos filtros dependentes
            var fds = filtros.FindAll(f => f.PerfisEntrada.Perfis.First().Id == perfil.Id);
            foreach (var filtro in fds)
            {
                // Add filtros dependentes diretos
                if (filtro.PerfisEntrada.Perfis.First().Id == perfil.Id && !filtrosDependentes.Contains(filtro))
                {
                    filtrosDependentes.Add(filtro);

                    if (filtrosDependentes.Count == filtros.Count)
                        return filtrosDependentes;
                }

                // Add filtros dependentes do filtrado
                var filtrosDepDoFiltrado = GetDependentFilters(filtro.PerfisSaída.Perfis.First(), filtros, cálculos);
                foreach (var filtroDF in filtrosDepDoFiltrado)
                {
                    if (!filtrosDependentes.Contains(filtroDF))
                        filtrosDependentes.Add(filtroDF);

                    if (filtrosDependentes.Count == filtros.Count)
                        return filtrosDependentes;
                }

                // Add filtros dependentes de cálculos dependentes do perfil filtrado
                var calcDeps = GetDependentCalculus(filtro.PerfisSaída.Perfis.First().ToString());
                foreach (var calcDep in calcDeps)
                {
                    var filtrosDeps = _pipelineManager.GetDependentFilters(calcDep);

                    foreach (var filtrosDep in filtrosDeps)
                    {
                        if (!filtrosDependentes.Contains(filtrosDep))
                            filtrosDependentes.Add(filtrosDep);

                        if (filtrosDependentes.Count == filtros.Count)
                            return filtrosDependentes;
                    }
                }
            }

            // Filtros dependentes dos cálculos dependentes
            var cálculosDependentes = _pipelineManager.GetDependentCalculus(perfil);
            foreach (var calcDep in cálculosDependentes)
            {
                var filtrosDeps = _pipelineManager.GetDependentFilters(calcDep);
                foreach (var filtrosDep in filtrosDeps)
                {
                    if (!filtrosDependentes.Contains(filtrosDep))
                        filtrosDependentes.Add(filtrosDep);

                    if (filtrosDependentes.Count == filtros.Count)
                        return filtrosDependentes;
                }
            }

            return filtrosDependentes;
        }
    }
}
