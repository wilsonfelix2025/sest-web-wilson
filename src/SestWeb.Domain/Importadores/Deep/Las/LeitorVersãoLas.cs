using SestWeb.Domain.Exceptions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Domain.Importadores.Deep.Las
{
    /// <summary>
    /// Classe responsável por, a partir de um conjunto de linhas, identificar a versão de
    /// um arquivo .LAS.
    /// </summary>
    public class LeitorVersãoLas
    {
        /// <summary>
        /// Pattern regex que identifica o formato da linha que contém a informação da versão do arquivo.
        /// </summary>
        private readonly string _patternVersão = @"VERS\s*\.\s*(?'version'(\d+)\.?(\d*))";

        /// <summary>
        /// Lista de versões suportadas pelo importador.
        /// </summary>
        private readonly List<double> _versõesSuportadas = new List<double> { 2.0, 3.0 };

        /// <summary>
        /// A versão obtida pelo extrator.
        /// </summary>
        public double Versão { get; }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="linhas">As linhas da seção de versão (tag ~V do arquivo).</param>
        public LeitorVersãoLas(string[] linhas)
        {
            // Extrai a versão das linhas recebidas.
            string versãoExtraída = ObterVersão(linhas);

            // Se o método de obter versão não encontrou a versão
            if (versãoExtraída == string.Empty)
            {
                throw new VersãoNãoEncontradaException("Não foi possível encontrar a versão do arquivo LAS recebido.");
            }

            Versão = StringUtils.ToDoubleInvariantCulture(versãoExtraída, 1);

            if (!_versõesSuportadas.Contains(Versão))
            {
                throw new VersãoNãoEncontradaException($"A importação de arquivos LAS com versão {Versão} não é suportada pela versão atual do SEST Web.");
            }
        }

        /// <summary>
        /// Extrai a versão do arquivo a partir das linhas recebidas.
        /// </summary>
        /// <param name="linhas">Array de linhas da seção demarcada pela tag ~V.</param>
        /// <returns>Uma string contendo a versão do arquivo se existir, uma string vazia caso contrário.</returns>
        private string ObterVersão(string[] linhas)
        {
            foreach (string linha in linhas)
            {
                // Tenta encontrar o padrão de versão na linha atual
                Match m = Regex.Match(linha, _patternVersão, RegexOptions.IgnoreCase);

                if (m.Success)
                {
                    return m.Groups["version"].Value;
                }
            }

            // Se não achar, retorna uma string vazia.
            return string.Empty;
        }
    }
}
