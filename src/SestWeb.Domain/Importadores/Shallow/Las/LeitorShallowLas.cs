using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Importadores.Deep.Las;

namespace SestWeb.Domain.Importadores.Shallow.Las
{
    public class LeitorShallowLas 
    {
        /// <summary>
        /// O caminho do arquivo atualmente sendo lido.
        /// </summary>
        private readonly string _caminhoArquivo;

        /// <summary>
        /// Leitor auxiliar que divide as linhas lidas em seções do arquivo LAS.
        /// </summary>
        private readonly LeitorSeçõesLas _leitorSeções = new LeitorSeçõesLas();

        /// <summary>
        /// Leitor auxiliar que obtém a lista de todas as curvas lidas do arquivo LAS.
        /// </summary>
        private LeitorCurvasLas _leitorCurvas;

        /// <summary>
        /// Classe representando um objeto de retorno informando os dados extraídos do arquivo LAS.
        /// </summary>
        private DadosLidos _dadosLidos = null;

        public LeitorShallowLas(string caminhoArquivo)
        {
            _caminhoArquivo = caminhoArquivo;
        }

        public RetornoDTO LerDados()
        {
            try
            {
                // Inicializa as flags importantes do fluxo de leitura shallow
                var leuVersão = false;
                var leuCurvas = false;

                foreach (var linha in File.ReadLines(_caminhoArquivo))
                {
                    // Para cada linha, obtém um resultado de leitura
                    var result = _leitorSeções.ProcessaLinha(linha);

                    if (result.Tipo == LeitorLinhaLasResultType.SeçãoConcluída)
                    {
                        // Se a seção concluída foi a de versão
                        if (result.Seção.Equals("~V"))
                        {
                            // Se o leitor for instanciado com sucesso, a versão foi obtida e é válida
                            var _leitorVersão = new LeitorVersãoLas(_leitorSeções.ObterSeção("~V"));
                            // Marca a flag de leitura de versão
                            leuVersão = true;
                        }
                        if (result.Seção.Equals("~C"))
                        {
                            // Se a seção concluída foi a de curvas, obtém as curvas do arquivo
                            _leitorCurvas = new LeitorCurvasLas(_leitorSeções.ObterSeção("~C"));
                            // Marca a flag de leitura de curvas
                            leuCurvas = true;
                        }
                        if (leuVersão && leuCurvas)
                        {
                            // Se as duas flags foram ativadas, inicia a preparação dos dados de retorno
                            List<RetornoPerfis> perfis = new List<RetornoPerfis>();

                            foreach (var mnemônico in _leitorCurvas.Perfis)
                            {
                                try
                                {
                                    // Obtem o tipo do perfil a partir do mnemônico e o insere na lista de perfis
                                    var unidadePerfil = _leitorCurvas.Informações[mnemônico];
                                    var tipoPerfil = TiposPerfil.GeTipoPerfil(_leitorCurvas.ObterMnemônicoOriginal(mnemônico));
                                    perfis.Add(new RetornoPerfis(mnemônico, tipoPerfil.Mnemônico, unidadePerfil.Símbolo));
                                }
                                catch (Exception)
                                {
                                    // Se não for possível obter o tipo do perfil, ele é adicionado sem tipo e sem unidade
                                    perfis.Add(new RetornoPerfis(mnemônico, "", ""));
                                }
                            }

                            // Monta o objeto de retorno
                            _dadosLidos = new DadosLidos
                            {
                                TemTrajetória = _leitorCurvas.TemTrajetória,
                                TemSapatas = false,
                                TemObjetivos = false,
                                TemEstratigrafia = false,
                                TemLitologia = _leitorCurvas.Litologias.Count() > 0,
                                Litologias = _leitorCurvas.Litologias,
                                Perfis = perfis
                            };
                            break;
                        }
                    }
                }

                if (!leuCurvas)
                {
                    throw new InvalidDataException("O arquivo recebido não contém informação de curvas.");
                }

                return _dadosLidos;
            }
            catch (FileNotFoundException)
            {   
                throw new FileNotFoundException($"Arquivo não encontrado: {_caminhoArquivo}");
            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }            
        }
    }
}
