using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines
{
    public class TrendDependenceManager
    {
        private readonly IPipelineManager _pipelineManager;

        public TrendDependenceManager(IPipelineManager pipelineManager)
        {
            _pipelineManager = pipelineManager;
        }

        public List<ICálculo> GetDependentCalculus(Trend.Trend trend, List<PerfilBase> perfisDoPoço)
        {
            List<ICálculo> cálculosDependentesExcetoGradientes = new List<ICálculo>();

            try
            {
                var perfil = perfisDoPoço?.Find(p => (p.TemTrendCompactação || p.TemTrendLBF) && p.Trend.Id.Equals(trend.Id));

                if (perfil == null)
                    return cálculosDependentesExcetoGradientes;

                var cálculosDependentesDoPerfilGerador = _pipelineManager.GetDependentCalculus(perfil);

                List<ICálculo> cálculosDependentesPressãoPoros = new List<ICálculo>();
                foreach (var cálculoDependente in cálculosDependentesDoPerfilGerador)
                {
                    if (cálculoDependente.GrupoCálculo == GrupoCálculo.PressãoPoros && cálculosDependentesPressãoPoros.Find(c => c.Id.Equals(cálculoDependente.Id)) == null)
                    {
                        foreach (var id in cálculoDependente.PerfisEntrada.IdPerfis)
                        {
                            var perfilDoCálculo = perfisDoPoço.Find(p => p.Id.ToString() == id);
                            if (perfilDoCálculo.ÉPerfilCalculado() && (perfilDoCálculo.TemTrendCompactação || perfilDoCálculo.TemTrendLBF))
                                cálculosDependentesPressãoPoros.Add(cálculoDependente);
                        }                        
                    }
                }

                cálculosDependentesExcetoGradientes.AddRange(cálculosDependentesPressãoPoros);

                foreach (var cálculoPressãoPoros in cálculosDependentesPressãoPoros)
                {
                    var cálculosDependente = _pipelineManager.GetDependentCalculus(cálculoPressãoPoros);

                    foreach (var cálculodependente in cálculosDependente)
                    {
                        if (cálculodependente.GrupoCálculo != GrupoCálculo.Gradientes && cálculosDependentesExcetoGradientes.Find(c => c.Id.Equals(cálculodependente.Id)) == null)
                            cálculosDependentesExcetoGradientes.Add(cálculodependente);
                    }
                }

                return cálculosDependentesExcetoGradientes;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na obtenção dos cálculos dependentes do trend : " + exception.Message);
            }
        }

        public List<IFiltro> GetDependentFilters(Trend.Trend trend, List<PerfilBase> perfisDoPoço)
        {
            List<IFiltro> filtrosDependentes = new List<IFiltro>();

            var cálculosDependentes = GetDependentCalculus(trend, perfisDoPoço);

            foreach (var calcDep in cálculosDependentes)
            {
                var filtrosDependentesDoCálculo = _pipelineManager.GetDependentFilters(calcDep);

                foreach (var filtrosDep in filtrosDependentesDoCálculo)
                {
                    if (!filtrosDependentes.Contains(filtrosDep))
                    {
                        filtrosDependentes.Add(filtrosDep);

                        if (filtrosDependentes.Count == _pipelineManager.FiltrosDoPoço.Count)
                            return filtrosDependentes;
                    }
                }
            }

            return filtrosDependentes;
        }

        public void InvalidateDependentCalculus(Trend.Trend trend, List<PerfilBase> perfisDoPoço)
        {
            var cálculosDependentes = GetDependentCalculus(trend, perfisDoPoço);
            _pipelineManager.InvalidateDependentCalculus(cálculosDependentes);
        }
    }
}
