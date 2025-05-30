using System.Collections.Generic;
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Domain.Importadores.Deep.Las
{
    internal class LeitorPoçoLas : LeitorHelperLas
    {
        /// <summary>
        /// Dicionário com informações do poço.
        /// </summary>
        public Dictionary<string, string> Informações { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Construtor da classe LeitorDeInformaçõesDoPoço.
        /// </summary>
        /// <param name="linhas">Linhas da seção do arquivo com informações do poço.</param>
        public LeitorPoçoLas(string[] linhas)
        {
            CarregarInformações(linhas);
        }

        /// <summary>
        /// Obtem o valor do mnemônico no dicionário de informações, se existir.
        /// </summary>
        /// <param name="mnemônico">O mnemônico a ser buscado no poço.</param>
        /// <returns>O dado associado ao mnemônico informado.</returns>
        public string ObterValor(string mnemônico)
        {
            Informações.TryGetValue(mnemônico.ToUpper(), out string valor);
            return valor;
        }

        /// <summary>
        /// Carrega informações referentes ao poço (mnemônico e o dado associado).
        /// </summary>
        /// <param name="linhas">Linhas da seção referente ao poço (~W) no arquivo .las.</param>
        private void CarregarInformações(string[] linhas)
        {
            foreach (string linha in linhas)
            {
                string mnemônico = ObterMnemônico(linha);
                string dado = ObterDadoDaLinha(linha);

                if (StringUtils.HasContent(mnemônico))
                {
                    continue;
                }

                if (!Informações.ContainsKey(mnemônico.ToUpper()))
                {
                    Informações.Add(mnemônico.ToUpper(), dado);
                }
            }
        }

    }
}
