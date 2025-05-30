using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Correlações.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Composto.Correlação;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;

namespace SestWeb.Domain.Entities.Cálculos.Perfis
{
    public class GerenciadorDeExpressõesDoCálculoDePerfis : IGerenciadorExpressãoDeCálculo
    {
        private readonly IList<ICorrelação> _correlações;
        private readonly IList<TrechoCálculo> _trechosDeCálculos;
        private readonly string _separador = ",";

        public GerenciadorDeExpressõesDoCálculoDePerfis(IList<ICorrelação> correlações, IList<TrechoCálculo> trechosDeCálculos)
        {
            _correlações = correlações;
            _trechosDeCálculos = trechosDeCálculos;
        }

        public string Extrair()
        {
            string expressão = string.Empty;

            if (_correlações != null)
                expressão = ObterExpressãoDasCorrelaçõesBase(_correlações);

            if (_trechosDeCálculos == null || !_trechosDeCálculos.Any()) return expressão;

            var correlaçõesDosTrechosOrdenadas = OrdenarCorrelaçõesDosTrechos();
            expressão += _separador + Environment.NewLine;
            foreach (var correlaçãoDeTrecho in correlaçõesDosTrechosOrdenadas)
            {
                var trechosDaCorrelaçãoOrdenadosPorTopo = _trechosDeCálculos.Where(t => t.Correlação.Nome == correlaçãoDeTrecho.Nome).OrderBy(t => t.Topo);
                if (trechosDaCorrelaçãoOrdenadosPorTopo == null || !trechosDaCorrelaçãoOrdenadosPorTopo.Any()) continue;
                var correlaçãoBaseDosTrechos = _correlações.First(c =>
                    c.PerfisSaída.Tipos.First() ==
                    trechosDaCorrelaçãoOrdenadosPorTopo.First().Correlação.PerfisSaída.Tipos.First());
                var expressãoComTrechos = ObterExpressãoComTrechos(correlaçãoBaseDosTrechos, trechosDaCorrelaçãoOrdenadosPorTopo.ToList());
                expressão += expressãoComTrechos;

                if (correlaçãoDeTrecho != correlaçõesDosTrechosOrdenadas.Last())
                {
                    expressão += _separador + Environment.NewLine;
                }
            }
            return expressão;
        }

        private IList<ICorrelação> OrdenarCorrelaçõesDosTrechos()
        {
            List<Correlação> correlaçõesDosTrechos = new List<Correlação>();
            foreach (var trechoDeCálculo in _trechosDeCálculos)
            {
                if (!correlaçõesDosTrechos.Exists(c => c.Nome == trechoDeCálculo.Correlação.Nome))
                    correlaçõesDosTrechos.Add(trechoDeCálculo.Correlação);
            }

            OrdenadorDeCorrelações ordenadorDeCorrelações = new OrdenadorDeCorrelações(correlaçõesDosTrechos, _separador);
            var correlaçõesDosTrechosOrdenadas = ordenadorDeCorrelações.OrdenarCorrelações();
            return correlaçõesDosTrechosOrdenadas;
        }

        private string ObterExpressãoDasCorrelaçõesBase(IEnumerable<ICorrelação> correlações)
        {
            OrdenadorDeCorrelações ordenadorDeCorrelações = new OrdenadorDeCorrelações(correlações, _separador);
            var correlaçõesOrdenadas = ordenadorDeCorrelações.OrdenarCorrelações();
            MontadorDeExpressões montadorDeExpressões = new MontadorDeExpressões(correlaçõesOrdenadas, _separador);
            return montadorDeExpressões.MontarExpressão();
        }

        private string ObterExpressãoComTrechos(ICorrelação correlaçãoBase, List<TrechoCálculo> trechosDeCálculos)
        {
            const string separador = ",";
            const string FAZ_NADA = "0";
            var expressão = string.Empty;
            var profundidade = Tokens.TokenDeCálculoPorTrecho;

            if (trechosDeCálculos == null || trechosDeCálculos.Count == 0) return correlaçãoBase.Expressão.Bruta;

            for (int index = 0; index < trechosDeCálculos.Count; index++)
            {
                var trecho = trechosDeCálculos[index];
                var trechoPossuiMesmaCorrelaçãoBaseComVariáveisInalteradas = TrechoPossuiMesmaCorrelaçãoBaseComVariáveisInalteradas(trecho, correlaçãoBase);
                if (trechoPossuiMesmaCorrelaçãoBaseComVariáveisInalteradas)
                {
                    continue;
                }

                var expressãoComParâmetrosInicializados = InicializarParâmetrosNaExpressão(trecho);

                // pula uma linha
                expressão += Environment.NewLine;
                expressão += string.Format(@"({0} >= {1} && {0} < {2}) ?
{5} = {6},
 {3} : {4}", profundidade, trecho.Topo,
                    trecho.Base, expressãoComParâmetrosInicializados, FAZ_NADA, trecho.Correlação.PerfisSaída.Tipos.First(), "0");

                if (index != trechosDeCálculos.Count - 1)
                {
                    expressão += separador;
                }
            }

            return expressão;
        }

        private string InicializarParâmetrosNaExpressão(TrechoCálculo trecho)
        {
            var expressão = trecho.Correlação.Expressão.Bruta;
            var pattern = @"\bvar\b\s*(?<Nome>\b[a-zA-Z]\w*)\s*=\s*(?<Valor>\d+\.?\d*)";
            var regex = new Regex(pattern);
            expressão = regex.Replace(expressão, match => ReplaceValorDaVariável("Valor", match, trecho));
            return expressão;
        }

        private string ReplaceValorDaVariável(string groupName, Match match, TrechoCálculo trecho)
        {
            var variável = match.Groups["Nome"].Value;
            var novoValor = trecho.Parâmetros[variável].ToString(CultureInfo.InvariantCulture);

            var capturado = match.Value;
            capturado = capturado.Remove(match.Groups[groupName].Index - match.Index, match.Groups[groupName].Length);
            capturado = capturado.Insert(match.Groups[groupName].Index - match.Index, novoValor);
            return capturado;
        }

        private bool TrechoPossuiMesmaCorrelaçãoBaseComVariáveisInalteradas(TrechoCálculo trecho, ICorrelação correlaçãoBase)
        {
            if (!trecho.Correlação.Nome.Equals(correlaçãoBase.Nome) ||
                !trecho.Correlação.PerfisSaída.Equals(correlaçãoBase.PerfisSaída))
            {
                return false;
            }

            foreach (var parâmetro in trecho.Parâmetros)
            {
                if (!correlaçãoBase.Expressão.Variáveis.Exists(parâmetro.Key))
                {
                    return false;
                }

                var valorOriginalExiste = correlaçãoBase.Expressão.Variáveis.TryGetValue(parâmetro.Key, out double valorOriginal);
                var valorModificado = parâmetro.Value;
                if (!valorOriginal.Equals(valorModificado))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
