using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Composto.Correlação
{
    public class AnalizadorDeDependências : IAnalizadorDeDependências
    {
        enum EstadoDaVisita
        {
            NãoVisitado,
            Visitando,
            Visitado
        }

        private readonly Dictionary<string, EstadoDaVisita> _visitas;

        public AnalizadorDeDependências(IEnumerable<ICorrelação> correlações)
        {
            Dependências = new Dictionary<string, List<string>>();
            DependênciasDiretas = new Dictionary<string, List<string>>();
            Ciclos = new List<List<string>>();
            _visitas = new Dictionary<string, EstadoDaVisita>();
            CarregarDependênciasDiretas(correlações);
        }

        public Dictionary<string, List<string>> Dependências { get; }
        public Dictionary<string, List<string>> DependênciasDiretas { get; }
        public List<List<string>> Ciclos { get; }

        private void CarregarDependênciasDiretas(IEnumerable<ICorrelação> correlações)
        {
            foreach (var correlação in correlações)
            {
                if(correlação==null)
                    continue;

                List<string> entradas;
                IdentificarDepedênciasDiretas(correlação.Expressão.Bruta, out entradas);
                RegistrarDependênciasDiretas(entradas, correlação.PerfisSaída.Tipos);
            }
        }

        private void IdentificarDepedênciasDiretas(string expressão, out List<string> entradas)
        {
            var tipos = TiposPerfil.TodosString;

            if (string.IsNullOrEmpty(expressão) || string.IsNullOrWhiteSpace(expressão))
            {
                entradas = null;
                return;
            }

            entradas = tipos.Where(tipo =>
            {
                string pattern = $@"\b{tipo}\b";
                var regex = new Regex(pattern);
                string saídaPattern = $@"\b{tipo}\b\s*=\s*";
                var regexSaída = new Regex(saídaPattern);
                return regex.IsMatch(expressão) && !regexSaída.IsMatch(expressão);
            }).ToList();
        }

        private void RegistrarDependênciasDiretas(List<string> entradas, IEnumerable<string> saídas)
        {
            if (entradas == null) return;

            foreach (var saída in saídas)
            {
                if (DependênciasDiretas.ContainsKey(saída))
                {
                    DependênciasDiretas[saída].AddRange(entradas);
                }
                else
                {
                    DependênciasDiretas.Add(saída, entradas);
                }
            }
        }

        private EstadoDaVisita ObterEstadoDaVisita(Dictionary<string, EstadoDaVisita> visited, string key, EstadoDaVisita defaultValue)
        {
            EstadoDaVisita value;
            if (visited.TryGetValue(key, out value))
                return value;
            return defaultValue;
        }

        private void MapearDependências(string perfil, Func<string, IEnumerable<string>> dependênciasDiretas, List<string> ancestrais)
        {
            var state = ObterEstadoDaVisita(_visitas, perfil, EstadoDaVisita.NãoVisitado);
            if (state == EstadoDaVisita.Visitado)
            {
                return;
            }
            else if (state == EstadoDaVisita.Visitando)
            {
                RegistrarCiclo(perfil, ancestrais);
            }
            else
            {
                _visitas[perfil] = EstadoDaVisita.Visitando;
                ProcessarDependências(perfil, dependênciasDiretas, ancestrais);
                _visitas[perfil] = EstadoDaVisita.Visitado;
            }
        }

        private void ProcessarDependências(string perfil, Func<string, IEnumerable<string>> dependênciasDiretas, List<string> ancestrais)
        {
            ancestrais.Add(perfil);

            foreach (var ancestral in ancestrais)
            {
                if (Dependências.ContainsKey(ancestral) && !Dependências[ancestral].Contains(perfil))
                {
                    AdicionarDependênciaInternal(ancestral, perfil);
                }
                else if (!Dependências.ContainsKey(ancestral))
                {
                    // adiciona as dependências diretas
                    if (DependênciasDiretas.ContainsKey(ancestral))
                    {
                        AdicionarDependênciasInternal(ancestral, DependênciasDiretas[ancestral].ToList());

                        // se o perfil não estava na lista de dependências diretas, o adiciona.
                        if (ancestral != perfil && !DependênciasDiretas[ancestral].Contains(perfil))
                        {
                            AdicionarDependênciasInternal(ancestral, new List<string> { perfil });
                        }
                    }
                    else // dependências diretas não contém ancestral
                    {
                        if (ancestral == perfil)
                        {
                            AdicionarDependênciasInternal(ancestral, new List<string>());
                        }
                        else
                        {
                            AdicionarDependênciasInternal(ancestral, new List<string> { perfil });
                        }
                    }
                }
            }

            foreach (var child in dependênciasDiretas(perfil))
            {
                MapearDependências(child, dependênciasDiretas, ancestrais);
            }
            ancestrais.RemoveAt(ancestrais.Count - 1);
        }


        private void AdicionarDependênciasInternal(string dependente, List<string> dependências)
        {
            if (!Dependências.ContainsKey(dependente))
            {
                Dependências.Add(dependente, new List<string>());
            }
            foreach (var dependência in dependências)
            {
                if (!Dependências[dependente].Contains(dependência))
                {
                    AdicionarDependênciaInternal(dependente, dependência);
                }
            }
        }

        private void AdicionarDependênciaInternal(string dependente, string dependência)
        {
            Dependências[dependente].Add(dependência);

            if (Dependências.ContainsKey(dependência))
            {
                foreach (var depDoDependente in Dependências[dependência])
                {
                    if (!Dependências[dependente].Contains(depDoDependente))
                    {
                        Dependências[dependente].Add(depDoDependente);
                    }
                }
            }

            foreach (var dep in Dependências)
            {
                if (dep.Value.Contains(dependente) && !dep.Value.Contains(dependência))
                {
                    AdicionarDependênciaInternal(dep.Key, dependência);
                }
            }
        }

        private void RegistrarCiclo(string perfil, List<string> ancestrais)
        {
            Ciclos.Add(ancestrais.Concat(new[] { perfil })
                .SkipWhile(parent => !EqualityComparer<string>.Default.Equals(parent, perfil)).ToList());
        }

        private void AnalizarDependências(Func<string, IEnumerable<string>> dependênciasDiretas)
        {
            foreach (var perfil in DependênciasDiretas.Keys)
            {
                MapearDependências(perfil, dependênciasDiretas, new List<string>());
            }
        }

        public void AnalizarDependências()
        {
            AnalizarDependências(key => DependênciasDiretas.ContainsKey(key) ? DependênciasDiretas[key] : Enumerable.Empty<string>());
        }
    }
}
