using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using static System.Threading.Tasks.Parallel;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines
{
    public class PipelinesExecutor : IPipelinesExecutor
    {
        private readonly IPipelineManager _pipelineManager;

        public PipelinesExecutor(List<ICálculo> cálculosDoPoço, List<IFiltro> filtrosDoPoço)
        {
            _pipelineManager = new PipelineManager(cálculosDoPoço, filtrosDoPoço);
        }


        public List<ICálculo> GetDependentCalculus(ICálculo executedCalculus)
        {
            try
            {
                List<ICálculo> dependentCalculus = _pipelineManager.GetDependentCalculus(executedCalculus);
                return dependentCalculus;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na execução da pipeline : " + exception.Message);
            }
        }

        public IList<PerfilBase> Recalculate(List<ICálculo> dependentCalculus)
        {
            try
            {
                ApplyFilters(dependentCalculus);
                ExecuteDependents(dependentCalculus);
                return GetDependentsPerfis(dependentCalculus);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na execução da pipeline : " + exception.Message);
            }
        }

        public List<ICálculo> GetDependentCalculus(PerfilBase perfil)
        {
            try
            {
                List<ICálculo> dependentCalculus = _pipelineManager.GetDependentCalculus(perfil);
                return dependentCalculus;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na execução da pipeline : " + exception.Message);
            }
        }

        public List<ICálculo> GetDependentCalculus(ILitologia litology)
        {
            try
            {
                List<ICálculo> dependentCalculus = _pipelineManager.ObterCálculosDependentesDaLitologia(litology);
                return dependentCalculus;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na execução da pipeline : " + exception.Message);
            }
        }

        public List<ICálculo> GetDependentCalculus(Trend.Trend trend, List<PerfilBase> perfisDoPoço)
        {
            try
            {
                List<ICálculo> dependentCalculus = _pipelineManager.GetDependentCalculus(trend, perfisDoPoço);
                return dependentCalculus;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na execução da pipeline : " + exception.Message);
            }
        }

        private void ApplyFilters(IList<ICálculo> calculus)
        {
            var grupos = _pipelineManager.FiltrosDoPoço.Select(s => s.GrupoCálculo).ToList();
            
            var filters = calculus.Where(f => grupos.Contains(f.GrupoCálculo));

            if (filters.Any())
            {
                foreach (var filter in filters)
                {
                    filter.Execute(true);
                }
            }

        }

        private void ExecuteDependents(IList<ICálculo> calculus)
        {
            if (!calculus.Any())
                return;

            var perfisCalcs = new List<ICálculo>();
            var sobrCalcs = new List<ICálculo>();
            var expDCalcs = new List<ICálculo>();
            var pporosCalcs = new List<ICálculo>();
            var propMecCalcs = new List<ICálculo>();
            var tensCalcs = new List<ICálculo>();
            var gradCalcs = new List<ICálculo>();

            for (var index = 0; index < calculus.Count; index++)
            {
                var calc = calculus[index];

                switch (calc.GrupoCálculo)
                {
                    case GrupoCálculo.Perfis:
                        perfisCalcs.Add(calc);
                        break;
                    case GrupoCálculo.Sobrecarga:
                        sobrCalcs.Add(calc);
                        break;
                    case GrupoCálculo.PressãoPoros:
                        pporosCalcs.Add(calc);
                        break;
                    case GrupoCálculo.PropriedadesMecânicas:
                        propMecCalcs.Add(calc);
                        break;
                    case GrupoCálculo.Tensões:
                        tensCalcs.Add(calc);
                        break;
                    case GrupoCálculo.ExpoenteD:
                        expDCalcs.Add(calc);
                        break;
                    case GrupoCálculo.Gradientes:
                        gradCalcs.Add(calc);
                        break;
                }
            }

            Execute(perfisCalcs);
            Execute(sobrCalcs);
            Execute(expDCalcs);
            Execute(pporosCalcs);
            Execute(propMecCalcs);
            Execute(tensCalcs);
            Execute(gradCalcs);
        }

        private void Execute(List<ICálculo> calcs)
        {
            ForEach(calcs, calc =>
            {
                calc.Execute(true);
            });
        }

        private IList<PerfilBase> GetDependentsPerfis(List<ICálculo> calcs)
        {
            List<PerfilBase> perfis = new List<PerfilBase>();

            foreach (var calc in calcs)
            {
                foreach (var perfil in calc.PerfisSaída.Perfis)
                {
                    var addedPerfil = perfis.Find(p => p.Equals(perfil));

                    if (addedPerfil == null)
                    {
                        perfis.Add(perfil);
                    }
                }

            }

            return perfis;
        }
    }
}
