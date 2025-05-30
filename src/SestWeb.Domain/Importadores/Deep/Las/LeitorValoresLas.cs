using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Domain.Importadores.Deep.Las
{
    /// <summary>
    /// Classe responsável por, a partir de um conjunto de linhas, 
    /// importar os valores das curvas, definidas na seção demarcada
    /// pela tag ~A no arquivo .LAS.
    /// </summary>
    internal class LeitorValoresLas : LeitorHelperLas
    {
        /// <summary>
        /// Uma coleção de listas, onde cada lista representa todos os valores de uma
        /// curva em particular.
        /// </summary>
        public ObservableCollection<List<string>> Valores { get; private set; } = new ObservableCollection<List<string>>();

        /// <summary>
        /// Construtor padrão da classe.
        /// </summary>
        /// <param name="linhas">Array de linhas da seção demarcada pela tag ~A.</param>
        /// <param name="quantidadeCurvas">Quantidade de curvas definidas no arquivo.</param>
        public LeitorValoresLas(string[] linhas, int quantidadeCurvas)
        {
            ObterValores(linhas, quantidadeCurvas);
        }

        /// <summary>
        /// Obtem os valores para as curvas a partir das linhas recebidas.
        /// </summary>
        /// <param name="linhas">Array de linhas da seção demarcada pela tag ~A.</param>
        /// <param name="quantidadeCurvas">Quantidade de curvas definidas no arquivo.</param>
        private void ObterValores(string[] linhas, int quantidadeCurvas)
        {
            if (linhas.Length <= 0)
            {
                return;
            }

            // Garante que as linhas estão bem-formatadas
            linhas = AjustarLinhas(linhas, quantidadeCurvas);

            // Cria uma lista de strings para cada curva do arquivo
            for (var i = 0; i < quantidadeCurvas; i++)
            {
                Valores.Add(new List<string>());
            }

            // Define um contador para indicar em qual curva inserir cada valor lido
            var curvaAtual = 0;

            // Para cada linha recebida
            foreach (var linha in linhas)
            {
                // Quebra a linha em {quantidadeCurvas} valores
                string[] valores = linha.Split();

                // Para cada um dos valores da linha
                foreach (var valor in valores)
                {
                    if (StringUtils.HasContent(valor))
                    {
                        Valores[curvaAtual].Add(valor);
                        curvaAtual += 1;
                    }                    
                }

                // Após encerrar a leitura de todos os valores da linha, reseta o índice da curva atualmente sendo lida
                curvaAtual = 0;
            }
        }

        /// <summary>
        /// Remove espaços em branco e reformata as linhas, garantindo a consistência entre as curvas e os valores lidos,
        /// mesmo que o arquivo LAS sendo processado tenha quebrado informações de várias colunas em mais de uma linha.
        /// </summary>
        /// <param name="linhas">O array de strings representando as linhas a serem ajustadas</param>
        /// <param name="quantidadeCurvas">O número de curvas no arquivo LAS.</param>
        /// <returns>Um array de strings contendo as linhas bem-formatadas.</returns>
        private string[] AjustarLinhas(string[] linhas, int quantidadeCurvas)
        {
            // Junta todas as linhas lidas em uma única string para remover todas as quebras de linha
            string infos = string.Join(" ", linhas);
            // Substitui tabulações por espaços
            infos = infos.Replace('\t', ' ');
            // Quebra a string anterior removendo valores vazios para obter apenas as posições com valores
            var infosArray = infos.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            // Seleciona as posições do array que tem conteúdo e remove espaços duplicados
            infosArray = infosArray.Where(x => !string.IsNullOrWhiteSpace(x)).Select(s => s.Trim()).ToArray();

            // A variável abaixo é um alias para esclarecer a operação aparentemente arbitrária na linha de código seguinte
            var quantidadeDados = infosArray.Length;
            // Computa a quantidade de linhas de dados a partir da quantidade de células e de colunas
            var quantidadeLinhas = quantidadeDados / quantidadeCurvas;
            
            string[] copiaLinhas = new string[quantidadeLinhas];

            // O for abaixo itera sobre infosArray em blocos de tamanho {quantidadeCurvas},
            // montando as linhas e as inserindo no array copiaLinhas.
            Parallel.For(0, quantidadeLinhas, index =>
            {
                int índiceInicial = index * quantidadeCurvas;
                copiaLinhas[index] = string.Join(" ", infosArray, índiceInicial, quantidadeCurvas);
            });

            return copiaLinhas;
        }
    }
}
