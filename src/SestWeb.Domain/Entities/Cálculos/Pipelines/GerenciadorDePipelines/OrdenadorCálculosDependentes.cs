using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines
{
    public class OrdenadorCálculosDependentes : IOrdenadorCálculosDependentes
    {
        public List<ICálculo> Order(List<ICálculo> cálculosDependentes)
        {
            List<ICálculo> calcOrd = new List<ICálculo>();

            var resp = from calc in cálculosDependentes
                orderby calc.GrupoCálculo
                group calc by calc.GrupoCálculo into g
                select new { Grupo = g.Key, Calculos = g.ToList() };

            foreach (var a in resp)
            {
                var calcs = a.Calculos;
                Dictionary<ICálculo, List<ICálculo>> dependências = new Dictionary<ICálculo, List<ICálculo>>();

                foreach (var calc in calcs)
                {
                    List<ICálculo> depCalcs = new List<ICálculo>();
                    var otherCalcs = calcs.ToList();
                    otherCalcs.Remove(calc);

                    foreach (var perfil in calc.PerfisEntrada.IdPerfis)
                    {
                        foreach (var oCalc in otherCalcs)
                        {
                            if (oCalc.PerfisSaída.IdPerfis.Any(p => p == perfil))
                            {
                                if (!depCalcs.Contains(oCalc))
                                    depCalcs.Add(oCalc);
                            }
                        }
                    }

                    dependências.Add(calc, depCalcs);
                }

                var ordGroupCalcs = OrdenarDependências(dependências);
                foreach (var calculo in ordGroupCalcs)
                {
                    calcOrd.Add(calculo);
                }
            }
            return calcOrd;
        }

        private List<ICálculo> OrdenarDependências(Dictionary<ICálculo, List<ICálculo>> dependências)
        {
            List<ICálculo> ordenados = new List<ICálculo>();

            foreach (var calculo in dependências.Keys)
            {
                var dependentes = OrdenarDependências(dependências, calculo);
                foreach (var dependente in dependentes)
                {
                    var nãoIncluído = ordenados.Find(c => c.Id == dependente.Id) == null;
                    if (nãoIncluído)
                        ordenados.Add(dependente);
                }
            }
            return ordenados;
        }

        private List<ICálculo> OrdenarDependências(Dictionary<ICálculo, List<ICálculo>> dependências, ICálculo calculo)
        {
            List<ICálculo> ordenados = new List<ICálculo>();

            if (dependências[calculo].Count == 0 && ordenados.Find(c => c.Id == calculo.Id) == null)
                ordenados.Add(calculo);

            else
            {
                foreach (var depCalc in dependências[calculo])
                {
                    var dependentes = OrdenarDependências(dependências, depCalc);
                    foreach (var dependente in dependentes)
                    {
                        var nãoIncluído = ordenados.Find(c => c.Id == dependente.Id) == null;

                        if (nãoIncluído)
                            ordenados.Add(dependente);
                    }
                    ordenados.Add(calculo);
                }
            }
            return ordenados;
        }
    }
}
