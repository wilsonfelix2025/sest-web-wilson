using SestWeb.Domain.Entities.Correlações.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.Composto.Correlação
{
    public class OrdenadorDeCorrelações : IOrdenadorDeCorrelações
    {
        private readonly IEnumerable<ICorrelação> _correlações;
        private readonly string _separador;

        public OrdenadorDeCorrelações(IEnumerable<ICorrelação> correlações, string separador)
        {
            _correlações = correlações;
            _separador = separador;
        }

        public IList<ICorrelação> OrdenarCorrelações()
        {
            var analizadorDeDependências = new AnalizadorDeDependências(_correlações);
            analizadorDeDependências.AnalizarDependências();

            if (analizadorDeDependências.Ciclos.Count > 0)
            {
                var nomeDasCorrelações = ObterNomeDasCorrelações();
                var ciclo = ObterSequênciaDoCiclo(analizadorDeDependências.Ciclos);
                throw new ArgumentException($"Referência circular encontrada na combinação das correlações ({nomeDasCorrelações}) - Ciclo [{ciclo}] ");
            }

            var dependências = analizadorDeDependências.Dependências;
            OrdenadorDeDependências ordenadorDeDependências = new OrdenadorDeDependências(dependências);
            var dependênciasOrdenadas = ordenadorDeDependências.OrdenarDependências();

            var correlaçõesOrdenadas = Ordenar(dependênciasOrdenadas);
            return correlaçõesOrdenadas;

        }

        private object ObterSequênciaDoCiclo(List<List<string>> ciclos)
        {
            return string.Join(" -> ", ciclos[0]);
        }

        private string ObterNomeDasCorrelações()
        {
            return string.Join("/", _correlações.Select(c => c.Nome));
        }

        private List<ICorrelação> Ordenar(List<string> dependênciasOrdenadas)
        {
            var correlaçõesOrdenadas = new List<ICorrelação>();
            foreach (var dependência in dependênciasOrdenadas)
            {
                var corr = _correlações.Where(c => c.PerfisSaída.Tipos.First() == dependência);
                if (corr == null || !corr.Any())
                    continue;

                if (corr.Count() > 1)
                {
                    corr = OrdenarCorrelaçõesComMesmaSaída(corr.ToList(), dependênciasOrdenadas);
                }
                correlaçõesOrdenadas.AddRange(corr);
            }
            return correlaçõesOrdenadas;
        }

        private List<ICorrelação> OrdenarCorrelaçõesComMesmaSaída(List<ICorrelação> correlações, List<string> dependênciasOrdenadas)
        {
            var score = new Dictionary<ICorrelação, int>();
            foreach (var correlação in correlações)
            {
                var depScore = 0;
                for (var index = 0; index < dependênciasOrdenadas.Count; index++)
                {
                    var dependência = dependênciasOrdenadas[index];

                    if (correlação.PerfisEntrada.Tipos.Contains(dependência))
                        depScore += index;
                }
                score.Add(correlação, depScore);
            }

            var correlaçõesOrdenadas = new List<ICorrelação>();
            foreach (var kvp in score.OrderBy(kvp => kvp.Value))
            {
                correlaçõesOrdenadas.Add(kvp.Key);
            }

            return correlaçõesOrdenadas;
        }

        private string MontarExpressão(List<string> dependênciasOrdenadas)
        {
            string expressão = string.Empty;

            foreach (var dependência in dependênciasOrdenadas)
            {
                var corr = _correlações.FirstOrDefault(c => c.PerfisSaída.Tipos.First() == dependência);
                if (corr == null)
                    continue;
                expressão += Environment.NewLine + corr.Expressão;

                if (dependência != dependênciasOrdenadas.Last())
                    expressão += _separador;
            }
            return expressão;
        }
    }
}
