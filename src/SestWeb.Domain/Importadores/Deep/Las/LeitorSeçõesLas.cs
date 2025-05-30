using System.Collections.Generic;
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Domain.Importadores.Deep.Las
{
    public class LeitorSeçõesLas
    {
        // TODO mover isso para o validador de seções
        private readonly List<string> _tagsSuportadas = new List<string>() { "~V", "~W", "~C", "~A" };

        /// <summary>
        /// A tag do arquivo LAS lida antes da atual. Essa informação é importante para o leitor shallow do
        /// arquivo LAS.
        /// </summary>
        private string _tagSeçãoAnterior = string.Empty;

        /// <summary>
        /// A tag do arquivo LAS atualmente sendo lida. Toda vez que um novo marcador de seção é identificado,
        /// esse atributo é atualizado.
        /// </summary>
        private string _tagSeçãoAtual = string.Empty;

        /// <summary>
        /// Lista de strings que atua como buffer, armazenando todas as linhas lidas da seção até o momento
        /// de fazer o salvamento no dicionário _seções.
        /// </summary>
        private readonly List<string> _linhasSeçãoAtual = new List<string>();

        /// <summary>
        /// Dicionário com todas as seções do arquivo LAS, onde a key é a tag da seção, e o value é a
        /// lista de linhas da seção.
        /// </summary>
        private readonly Dictionary<string, string[]> _seções = new Dictionary<string, string[]>();

        /// <summary>
        /// Método principal da classe. Recebe um conjunto de linhas representando todo um arquivo .LAS e
        /// separa ele em seções de acordo com a tag delimitando cada uma delas.
        /// </summary>
        /// <param name="linhas">Um conjunto de linhas representando todo ou parte de um arquivo LAS.</param>
        public void SepararEmSeções(string[] linhas)
        {            
            foreach (var linha in linhas)
            {
                ProcessaLinha(linha);
            }

            // É necessário salvar a seção atual para salvar a seção atualmente sendo lida no dicionário e encerrar a leitura
            SalvarSeçãoAtual();
        }

        /// <summary>
        /// Examina uma linha do arquivo LAS, emitindo um resultado para cada uma delas.
        /// </summary>
        /// <remarks>
        /// O principal propósito de isso ser feito dessa forma é que tanto o leitor deep quanto o shallow possam se
        /// beneficiar desse leitor. O leitor shallow tem uma característica mais iterativa, enquanto o leitor deep
        /// se comporta de uma forma mais bulk. Emitir um resultado a cada linha garante que o leitor shallow possa
        /// interromper a execução a qualquer momento caso algo de errado tenha acontecido, e também permite que ele
        /// verifique integridade de consistência de uma seção, por exemplo, assim que a leitura da seção for concluída,
        /// em vez de ter que aguardar a leitura de todo o arquivo. Logo, se um arquivo LAS contiver uma versão inválida,
        /// por exemplo, o leitor shallow consegue interromper a leitura na segunda linha do arquivo, em vez de ter que
        /// esperar a leitura de mais 50 mil linhas.
        /// </remarks>
        /// <param name="linha">A linha sendo examinada.</param>
        /// <returns>Um resultado de leitura de linha, informando o tipo do evento e a seção na qual o evento ocorreu.</returns>
        public LeitorLinhaLasResult ProcessaLinha(string linha)
        {            
            if (!StringUtils.HasContent(linha))
            {
                // Se a linha é vazia, notifica que a linha foi ignorada
                return new LeitorLinhaLasResult(LeitorLinhaLasResultType.LinhaIgnorada, _tagSeçãoAtual);
            }
            
            // Remove espaços desnecessários para evitar falsos negativos
            linha = linha.Trim();

            if (linha.StartsWith('#'))
            {
                return new LeitorLinhaLasResult(LeitorLinhaLasResultType.LinhaIgnorada, _tagSeçãoAtual);
            }

            // Quando uma nova seção for identificada
            if (linha.StartsWith('~'))
            {
                // Encerra a atual e inicia uma nova com a tag da seção recebida
                SalvarSeçãoAtual();
                IniciarNovaSeção(linha);
                return new LeitorLinhaLasResult(LeitorLinhaLasResultType.SeçãoConcluída, _tagSeçãoAnterior);
            }
            else
            {
                // Se nenhum identificador de seção foi atingido, ignora linhas até chegar em algum
                if (_tagSeçãoAtual == string.Empty)
                {
                    return new LeitorLinhaLasResult(LeitorLinhaLasResultType.LinhaIgnorada, _tagSeçãoAtual);
                }

                // Se já existe um identificador de seção, adiciona a linha lida ao buffer de linhas
                _linhasSeçãoAtual.Add(linha);
                return new LeitorLinhaLasResult(LeitorLinhaLasResultType.LinhaAdicionada, _tagSeçãoAtual);
            }
        }

        /// <summary>
        /// Método de acesso ao dicionário de seções.
        /// </summary>
        /// <param name="tag">A tag indicando a seção a ser obtida.</param>
        /// <returns>Um array de strings representando as linhas da seção.</returns>
        public string[] ObterSeção(string tag)
        {
            if (_seções.ContainsKey(tag))
            {
                return _seções[tag];
            }

            return null;
        }

        /// <summary>
        /// Configura os atributos da classe para o armazenamento de uma nova seção do arquivo LAS.
        /// </summary>
        /// <param name="linha">A linha que delimita o início de uma nova seção.</param>
        private void IniciarNovaSeção(string linha)
        {
            // Extrai a tag da linha
            string tag = linha.Substring(0, 2);
            // Como a seção está concluída, ela agora passa a ser a anterior
            _tagSeçãoAnterior = _tagSeçãoAtual;
            // Define a tag atualmente sendo lida
            _tagSeçãoAtual = tag;
            // Reinicia o buffer que vai armazenar as linhas da seção
            _linhasSeçãoAtual.Clear();
        }

        /// <summary>
        /// Persiste o conteúdo do buffer de linhas no dicionário de seções. 
        /// </summary>
        private void SalvarSeçãoAtual()
        {
            // Se a key não for fazia
            if (_tagSeçãoAtual != string.Empty)
            {
                // Cria uma nova seção com o conteúdo do buffer de linhas
                _seções[_tagSeçãoAtual] = _linhasSeçãoAtual.ToArray();
            }
        }
    }
}
