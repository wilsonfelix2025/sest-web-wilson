using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SestWeb.Domain.Importadores.Deep.Las
{
    internal abstract class LeitorHelperLas
    {
        private readonly string _padrãoMnemônico = @"(?'mnemonico'.*)\s*\.";
        private readonly string _padrãoUnidade = @"\.(?'unidade'\S*)";
        private readonly string _padrãoDado = @"(\w+\s*\.\w*)\s*(?'dado'.+)";

        /// <summary>
        /// Conversor de unidades.
        /// </summary>
        private Dictionary<string, string> _conversões = new Dictionary<string, string>()
        {
            { "1000lbf", "klbf"},
            { "1000ft.lbf", "ft.klbf" },
            { "gAPI", "API" }
        };

        /// <summary>
        /// Obtém o mnemônico da linha.
        /// </summary>
        /// <param name="linha">Linha lida do arquivo .las.</param>
        /// <returns>Retorna o mnemônico.</returns>
        protected string ObterMnemônico(string linha)
        {
            string mnemônico = string.Empty;
            string linhaFormatada = RemoverComentárioLas(linha);
            linhaFormatada = RemoverEspaçosDuplosETabs(linhaFormatada);

            Match m = Regex.Match(linhaFormatada, _padrãoMnemônico);

            if (m.Success)
            {
                mnemônico = m.Groups["mnemonico"].Value.Trim();
            }

            return mnemônico;
        }

        /// <summary>
        /// Obtem a unidade da linha.
        /// </summary>
        /// <param name="line">Linha lida do arquivo .las.</param>
        /// <returns>Retorna a unidade, caso exista na linha.</returns>
        protected string ObterUnidade(string linha)
        {
            string unidade = string.Empty;
            string linhaFormatada = RemoverComentárioLas(linha);
            linhaFormatada = RemoverEspaçosDuplosETabs(linhaFormatada);

            Match m = Regex.Match(linhaFormatada, _padrãoUnidade);

            if (m.Success)
            {
                unidade = m.Groups["unidade"].Value.Trim();

                foreach (var conversão in _conversões)
                {
                    // TODO otimizar isso tentando o acesso direto ao dicionário?
                    if (unidade.Contains(conversão.Key))
                    {
                        unidade = unidade.Replace(conversão.Key, conversão.Value);
                        break;
                    }
                }
            }

            return unidade;
        }

        /// <summary>
        /// Obtem o dado da linha, isto é, o que vem após a unidade (se houver), ou após o mnemônico,
        /// caso não haja unidade.
        /// </summary>
        /// <param name="linha">Linha lida do arquivo .las.</param>
        /// <returns>Retorna o dado da linha, caso exista.</returns>
        protected string ObterDadoDaLinha(string linha)
        {
            string dado = string.Empty;
            string linhaFormatada = RemoverComentárioLas(linha);
            linhaFormatada = RemoverEspaçosDuplosETabs(linhaFormatada);

            Match m = Regex.Match(linhaFormatada, _padrãoDado);

            if (m.Success)
            {
                dado = m.Groups["dado"].Value.Trim();
            }

            return dado;
        }

        /// <summary>
        /// Remove espaços múltiplos e tabs (\t).
        /// </summary>
        /// <param name="str">String para processar.</param>
        /// <returns>Retorna uma string sem espaços múltiplos e tabulações.</returns>
        protected string RemoverEspaçosDuplosETabs(string str)
        {
            // Retira tab (\t) e dois ou mais espaços
            return Regex.Replace(str, @"\s{1,}", " ").Trim();
        }

        /// <summary>
        /// Remove o comentário da linha obtida do arquivo LAS.
        /// </summary>
        /// <remarks>
        /// Esta função é um alias para o regex replace usado várias vezes no código, com o intuito
        /// de deixar a função do snippet mais clara.
        /// </remarks>
        /// <param name="linha"></param>
        /// <returns></returns>
        private string RemoverComentárioLas(string linha)
        {
            return Regex.Replace(linha, @"\:.+", string.Empty);
        }
    }
}
