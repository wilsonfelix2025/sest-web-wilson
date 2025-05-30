using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Composto.Correlação
{
    internal class OrdenadorDeDependências : IOrdenadorDeDependências
    {
        private Dictionary<string, List<string>> dependências;

        public OrdenadorDeDependências(Dictionary<string, List<string>> dependências)
        {
            this.dependências = dependências;
        }

        public List<string> OrdenarDependências()
        {
            List<string> ordenados = new List<string>();
            var grouped = dependências?.GroupBy(dep => dep.Value.Count).OrderBy(dep => dep.Key);
            Dictionary<int, List<string>> agrupadosPorDependentes = grouped?.ToDictionary(gdc => gdc.Key, gdc => gdc.Select(g => g.Key).ToList());

            if (agrupadosPorDependentes == null) return null;

            foreach (var apd in agrupadosPorDependentes)
            {
                ordenados.AddRange(apd.Value);
            }

            return ordenados;
        }
    }
}