using System.Collections.Generic;
using SestWeb.Domain.Importadores.Shallow.Utils;
using SestWeb.Domain.SistemasUnidades.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.Importadores.Deep.Las
{
    /// <summary>
    /// Classe para importação de informações das curvas, delimitadas pela tag ~C
    /// no arquivo .las.
    /// </summary>
    internal class LeitorCurvasLas : LeitorHelperLas
    {
        /// <summary>
        /// Dicionário que relaciona o mnemônico de um perfil à sua unidade de medida.
        /// </summary>
        public Dictionary<string, UnidadeMedida> Informações { get; private set; } = new Dictionary<string, UnidadeMedida>();

        /// <summary>
        /// Lista com todos os mnemônicos lidos. É usada devido à preservação da ordem de inserção.
        /// </summary>
        private List<string> _mnemônicosLidos = new List<string>();

        /// <summary>
        /// Dicionário para mapear um mnemônico gerado pelo leitor ao mnemônico original.
        /// </summary>
        private Dictionary<string, string> _mnemônicosOriginais = new Dictionary<string, string>();

        /// <summary>
        /// Dicionário usado para listar mnemônicos repetidos no arquivo, caso haja, e quantas vezes eles ocorrem.
        /// </summary>
        /// <remarks>
        /// Isso é particularmente útil quando o importador gera nomes únicos para curvas que aparecem mais de uma vez
        /// no arquivo LAS.
        /// </remarks>
        private Dictionary<string, int> _mnemônicosRepetidos = new Dictionary<string, int>();

        /// <summary>
        /// Lista com os mnemônicos das litologias que constam no arquivo.
        /// </summary>
        public List<string> Litologias = new List<string>();

        /// <summary>
        /// Lista com os mnemônicos dos perfis que constam no arquivo.
        /// </summary>
        public List<string> Perfis = new List<string>();

        /// <summary>
        /// Índice da coluna de profundidade medida na lista de curvas lidas.
        /// </summary>
        public int ÍndiceColunaPm { get; private set; } = -1;

        /// <summary>
        /// Índice da coluna de inclinação na lista de curvas lidas.
        /// </summary>
        public int ÍndiceColunaInclinação { get; private set; } = -1;

        /// <summary>
        /// Índice da coluna de azimute na lista de curvas lidas.
        /// </summary>
        public int ÍndiceColunaAzimute { get; private set; } = -1;

        /// <summary>
        /// Se todas as colunas referentes a trajetória estão presentes no arquivo lido.
        /// </summary>
        public bool TemTrajetória { get; private set; }

        /// <summary>
        /// Lista de mnemônicos suportados para cada coluna.
        /// </summary>
        private Dictionary<string, List<string>> _mnemônicosSuportados = new Dictionary<string, List<string>>()
        {
            { "PM", new List<string>() { "DEPT", "DEPTH", "MD" } },
            { "Inclinação", new List<string>() { "INCL", "INC" } },
            { "Azimute", new List<string>() { "AZIM", "AZI" } },
            { "Litologia", new List<string>() { "LITO", "AGP" } }
        };

        public LeitorCurvasLas(string[] linhas)
        {
            ObterCurvas(linhas);
        }

        /// <summary>
        /// Obtem quantas curvas foram lidas do arquivo .las.
        /// </summary>
        /// <returns>A quantidade de curvas lidas do arquivo.</returns>
        public int ObterNúmeroDeCurvas()
        {
            return Informações.Count;
        }

        /// <summary>
        /// Percorre as linhas da seção de curvas, armazenando o mnemônico e a unidade de cada curva.
        /// </summary>
        /// <param name="linhas">As linhas da seção de curvas.</param>
        private void ObterCurvas(string[] linhas)
        {            
            for (var i = 0; i < linhas.Length; i++)
            {
                // Cria um alias para evitar referência excessiva ao i ao longo do método
                string linha = linhas[i];
                linha = linha.Trim();

                string mnemônico = ObterMnemônico(linha);                

                // Se a linha não tiver conteúdo ou for um comentário
                if (!StringUtils.HasContent(linha))
                {
                    continue;
                }

                string simboloUnidade = ObterUnidade(linha);
                UnidadeMedida unidade = UnidadeMedida.ObterPorSímbolo(simboloUnidade);

                if (unidade == null)
                {
                    // Se não foi possível obter a unidade a partir do símbolo, usa a unidade sem dimensão como padrão
                    unidade = UnidadeMedida.ObterUnidade<SemDimensão>();
                }
                
                mnemônico = mnemônico.ToUpperInvariant();

                var mnemônicoOriginal = mnemônico;

                // Se a curva sendo processada já foi adicionada à lista de curvas
                if (Informações.ContainsKey(mnemônico))
                {
                    mnemônico = GerarNomeParaMnemônicoRepetido(mnemônico);
                }

                if (_mnemônicosSuportados["Litologia"].Contains(mnemônico))
                {
                    Litologias.Add(mnemônico);
                } 
                else if (_mnemônicosSuportados["PM"].Contains(mnemônico))
                {
                    ÍndiceColunaPm = i;
                } 
                else if (_mnemônicosSuportados["Inclinação"].Contains(mnemônico))
                {
                    ÍndiceColunaInclinação = i;
                } 
                else if (_mnemônicosSuportados["Azimute"].Contains(mnemônico))
                {
                    ÍndiceColunaAzimute = i;
                } 
                else
                {
                    // Se o mnemônico não é uma curva de trajetória nem de litologia, é de perfil
                    Perfis.Add(mnemônico);
                }

                if (ÍndiceColunaPm > -1 && ÍndiceColunaInclinação > -1 && ÍndiceColunaAzimute > -1)
                {
                    TemTrajetória = true;
                }

                _mnemônicosOriginais.Add(mnemônico, mnemônicoOriginal);
                _mnemônicosLidos.Add(mnemônico);
                Informações.Add(mnemônico, unidade);                
            }
        }

        /// <summary>
        /// Gera um nome diferente para curvas com o mesmo mnemônico.
        /// </summary>
        /// <param name="mnemônico">O mnemônico extraído do arquivo .las.</param>
        /// <returns>O mnemônico recebido, com um contador de ocorrências adicionado ao final.</returns>
        private string GerarNomeParaMnemônicoRepetido(string mnemônico)
        {
            // O que sabemos a priori é que o mnemônico ocorreu no arquivo pelo menos uma vez
            int ocorrênciasDoMnemônico = 1;
            string novoMnemônico = mnemônico;

            // Se esse mnemônico ainda não está na lista de mnemônicos repetidos
            if (!_mnemônicosRepetidos.ContainsKey(mnemônico))
            {
                // Esta é a segunda ocorrência dele
                ocorrênciasDoMnemônico = 2;
                _mnemônicosRepetidos.Add(mnemônico, ocorrênciasDoMnemônico);
            }
            else
            {
                // Do contrário, ele já ocorreu n vezes, e precisamos incrementar esse número
                ocorrênciasDoMnemônico = _mnemônicosRepetidos[mnemônico];
                _mnemônicosRepetidos[mnemônico] = ++ocorrênciasDoMnemônico;
            }

            // Appendamos o contador de ocorrências ao mnemônico para diferenciá-lo dos demais
            novoMnemônico += ocorrênciasDoMnemônico.ToString();

            return novoMnemônico;
        }

        /// <summary>
        /// Obtem o índice de um mnemônico na lista de mnemônicos.
        /// </summary>
        /// <param name="mnemônico">O mnemônico da curva a ser encontrada.</param>
        /// <returns>Zero ou um inteiro positivo representando a posição do mnemônico na lista; -1 se não encontrar.</returns>
        public int ObterÍndiceDoMnemônico(string mnemônico)
        {
            mnemônico = mnemônico.ToUpperInvariant().Trim();

            for (var i = 0; i < _mnemônicosLidos.Count; i++)
            {
                if (_mnemônicosLidos[i] == mnemônico)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Obtém qual era o mnemônico original da curva importada.
        /// </summary>
        /// <remarks>
        /// Se houver mais de uma curva com o mesmo mnemônico no arquivo LAS, é gerado um novo mnemônico, geralmente
        /// com um número no final. É necessário saber qual era o mnemônico original no fluxo de importação, pois os
        /// mnemônicos gerados não são mapeados para nenhum tipo de perfil; DTC3 não é um tipo de perfil conhecido,
        /// mas DTC é. Logo, é necessário ser possível voltar de DTC3 para DTC para obter as informações do perfil.
        /// </remarks>
        /// <param name="mnemônico">O mnemônico a ser consultado.</param>
        /// <returns>O mnemônico original da curva.</returns>
        public string ObterMnemônicoOriginal(string mnemônico)
        {
            return _mnemônicosOriginais[mnemônico];
        }
    }
}
