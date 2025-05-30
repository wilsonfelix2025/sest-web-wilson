using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.Cálculos.Tensões;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines
{
    public class CalculusDependenceManager
    {
        private readonly IPipelineManager _pipelineManager;
        private readonly IOrdenadorCálculosDependentes _ordenadorCálculos;

        public CalculusDependenceManager(IPipelineManager pipelineManager)
        {
            _pipelineManager = pipelineManager;
            _ordenadorCálculos = new OrdenadorCálculosDependentes();
        }

        public List<ICálculo> GetDependentCalculus(ICálculo cálculo)
        {
            var cálculosDependentes = GetDependentCalculus(cálculo, _pipelineManager.CálculosDoPoço, _pipelineManager.FiltrosDoPoço)
                .OrderBy(c => c.GrupoCálculo).ToList();
            return _ordenadorCálculos.Order(cálculosDependentes);
        }

        private List<ICálculo> GetDependentCalculus(ICálculo cálculoEmAnálise, List<ICálculo> cálculosDoPoço, List<IFiltro> filtros)
        {
            var cálculosRestantes = cálculosDoPoço.ToList();
            cálculosRestantes.Remove(cálculoEmAnálise);

            List<ICálculo> cálculosDependentes = ExecuteDependentFilters(cálculoEmAnálise, filtros, cálculosDoPoço);

            foreach (var perfil in cálculoEmAnálise.PerfisSaída.IdPerfis)
            {
                foreach (var calc in cálculosRestantes)
                {
                    var cálculoContémPerfil = calc.ContémPerfilEntradaPorId(perfil);
                    var depleçãoContémPerfil = VerificarDepleçãoContémPerfil(calc, perfil);

                    if (!cálculoContémPerfil && !depleçãoContémPerfil)
                        continue;

                    // adiciona o cálculo dependente 
                    var nãoIncluído = cálculosDependentes.Find(c => c.Id == calc.Id) == null;
                    if (nãoIncluído)
                        cálculosDependentes.Add(calc);

                    // Adiciona os dependentes do cálculo dependentes
                    var dependentes = GetDependentCalculus(calc, cálculosRestantes, filtros);
                    foreach (var dependente in dependentes)
                    {
                        nãoIncluído = cálculosDependentes.Find(c => c.Id == dependente.Id) == null;

                        if (nãoIncluído)
                            cálculosDependentes.Add(dependente);
                    }
                }
            }

            return cálculosDependentes;
        }

        public bool VerificarDepleçãoContémPerfil(ICálculo cálculo, string perfilId)
        {
            if (cálculo is CálculoTensões)
            {
                var cálculosTensões = (CálculoTensões)cálculo;
                var contémDepleção = cálculosTensões.Depleção != null;

                if (contémDepleção && cálculosTensões.Depleção.ContémPerfil(perfilId))
                    return true;
            }

            return false;
        }

        public void InvalidateDependentCalculus(List<ICálculo> cálculosDependentes)
        {
            foreach (var cálculo in cálculosDependentes)
            {
                cálculo.PerfisSaída.Perfis.ForEach(perfilSaída =>
                {
                    if (perfilSaída.Mnemonico != "AZTHmin")
                        perfilSaída.Clear();
                });
            }
        }

        public List<IFiltro> GetDependentFilters(ICálculo cálculo)
        {
            return GetDependentFilters(cálculo, _pipelineManager.FiltrosDoPoço, _pipelineManager.CálculosDoPoço);
        }

        private List<ICálculo> ExecuteDependentFilters(ICálculo cálculoExecutado, List<IFiltro> filtros, List<ICálculo> cálculosDoPoço)
        {
            List<ICálculo> cálculosDependentesDosFiltrosAplicados = new List<ICálculo>();

            if (filtros == null || filtros.Count == 0)
                return cálculosDependentesDosFiltrosAplicados;

            foreach (var perfilDeSaída in cálculoExecutado.PerfisSaída.IdPerfis)
            {
                var deps = _pipelineManager.ObterCálculosDependentesDosPerfisFiltradosDependentes(perfilDeSaída);

                foreach (var dependente in deps)
                {
                    var nãoIncluído = cálculosDependentesDosFiltrosAplicados.Find(c => c.Id == dependente.Id) == null;

                    if (nãoIncluído)
                        cálculosDependentesDosFiltrosAplicados.Add(dependente);
                }
            }

            return cálculosDependentesDosFiltrosAplicados;
        }

        private List<IFiltro> GetDependentFilters(ICálculo cálculoExecutado, List<IFiltro> filtros, List<ICálculo> cálculosDoPoço)
        {
            List<IFiltro> filtrosDependentes = new List<IFiltro>();

            if (filtros == null || filtros.Count == 0)
                return filtrosDependentes;

            // proteção para testes em cálculos carregados de arquivos antigos em que se permitia usar como entrada, uma sa´da do mesmo cálculo.
            foreach (var perfilDeEntrada in cálculoExecutado.PerfisEntrada.Perfis)
            {
                if (perfilDeEntrada.Nome.Equals(perfilDeEntrada.TipoPerfil.Mnemônico + "_" + cálculoExecutado.Nome))
                {
                    return filtrosDependentes;
                }
            }

            foreach (var perfilDeSaída in cálculoExecutado.PerfisSaída.Perfis)
            {
                var deps = _pipelineManager.GetDependentFilters(perfilDeSaída);

                foreach (var dependente in deps)
                {
                    var nãoIncluído = filtrosDependentes.Find(f => f.Id == dependente.Id) == null;
                    if (nãoIncluído)
                    {
                        filtrosDependentes.Add(dependente);

                        if (filtrosDependentes.Count == filtros.Count)
                            return filtrosDependentes;
                    }
                }

                // Add filtros dependentes de cálculos dependentes do perfil filtrado
                var calcDeps = _pipelineManager.GetDependentCalculus(perfilDeSaída);
                foreach (var calcDep in calcDeps)
                {
                    var filtrosDeps = GetDependentFilters(calcDep, filtros, cálculosDoPoço);

                    foreach (var filtrosDep in filtrosDeps)
                    {
                        if (!filtrosDependentes.Contains(filtrosDep))
                        {
                            filtrosDependentes.Add(filtrosDep);

                            if (filtrosDependentes.Count == filtros.Count)
                                return filtrosDependentes;
                        }
                    }
                }
            }

            return filtrosDependentes;
        }
    }
}
