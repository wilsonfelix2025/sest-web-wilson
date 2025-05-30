using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.Shallow.SestTR2;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{
    public class LeitorDeepSestTR2: LeitorDeepArquivo
    {
        private List<string> _nomesLitologiasSelecionadas = new List<string>();
        private List<string> _nomesPerfisSelecionados = new List<string>();
        private string _poçoSelecionado = "";

        private LeitorDadosGeraisTR2 _leitorDadosGerais;
        private LeitorTrajetóriaTR2 _leitorTrajetória;
        private LeitorDeepLitologiaTR2 _leitorLitologia;
        private LeitorDeepPerfisTR2 _leitorPerfis;
        private LeitorDeepSapatasTR2 _leitorSapatas;
        private LeitorDeepObjetivosTR2 _leitorObjetivos;
        private LeitorDeepEstratigrafiaTR2 _leitorEstratigrafia;
        private LeitorEventosTR2 _leitorEventos;
        private LeitorRegistrosTR2 _leitorRegistros;
        private bool lerPoço = false;

        public LeitorDeepSestTR2(string caminhoDoArquivo, List<DadosSelecionadosEnum> dadosSelecionados,
            List<PerfilParaImportarDTO> perfisSelecionados, List<LitologiaParaImportarDTO> litologiasSelecionadas, Dictionary<string, object> dadosExtras)
            : base(caminhoDoArquivo, dadosSelecionados, perfisSelecionados, litologiasSelecionadas, dadosExtras)
        {
            
        }

        public override bool TryGetTrajetória(out TrajetóriaDTO trajetória)
        {
            trajetória = _leitorTrajetória != null && _leitorTrajetória.Trajetória != null ? _leitorTrajetória.Trajetória : new TrajetóriaDTO();
            return true;
        }

        public override bool TryGetLitologias(out List<LitologiaDTO> litologias)
        {
            var nomesLitologiasSelecionadas = LitologiasSelecionadas.Select(lito => lito.Nome);
            litologias = _leitorLitologia.ObterLitologias();
            return true;
        }

        public override bool TryGetPerfis(out List<PerfilDTO> perfis)
        {
            var nomesPerfisSelecionados = PerfisSelecionados.Select(perfil => perfil.Nome);
            perfis = _leitorPerfis.ObterPerfis();
            return true;
        }

        public override bool TryGetSapatas(out List<SapataDTO> sapatas)
        {
            sapatas = _leitorSapatas.ObterSapatas();
            return true;
        }

        public override bool TryGetObjetivos(out List<ObjetivoDTO> objetivos)
        {
            objetivos = _leitorObjetivos.ObterObjetivos();
            return true;
        }

        public override bool TryGetDadosGerais(out DadosGeraisDTO dadosGerais)
        {
            dadosGerais = _leitorDadosGerais != null && _leitorDadosGerais.DadosGerais != null ? _leitorDadosGerais.DadosGerais : new DadosGeraisDTO();
            return true;
        }

        public override bool TryGetEstratigrafia(out EstratigrafiaDTO estratigrafia)
        {
            estratigrafia = _leitorEstratigrafia.ObterEstratigrafia();
            return true;
        }

        public override void FazerAçõesIniciais()
        {
            var streamReader = LeitorHelperTR2.AbrirArquivoSestTR2(CaminhoOuDadoDoArquivo);
            var poçoAtual = "";

            _poçoSelecionado = DadosExtras["PoçoSelecionado"].ToString();
            _nomesLitologiasSelecionadas = LitologiasSelecionadas.Select(lito => lito.Nome).ToList();
            _nomesPerfisSelecionados = PerfisSelecionados.Select(lito => lito.Nome).ToList();

            _leitorDadosGerais = new LeitorDadosGeraisTR2(_poçoSelecionado);
            _leitorTrajetória = new LeitorTrajetóriaTR2();
            _leitorLitologia = new LeitorDeepLitologiaTR2(_nomesLitologiasSelecionadas);
            _leitorPerfis = new LeitorDeepPerfisTR2(_nomesPerfisSelecionados);
            _leitorRegistros = new LeitorRegistrosTR2();
            _leitorSapatas = new LeitorDeepSapatasTR2();
            _leitorObjetivos = new LeitorDeepObjetivosTR2();
            _leitorEstratigrafia = new LeitorDeepEstratigrafiaTR2();

            ModoLeituraArquivoTR2 modoLeitura = ModoLeituraArquivoTR2.Nenhum;
            var i = 0;

            try
            {
                while (!streamReader.EndOfStream)
                {
                    var linha = streamReader.ReadLine();

                    // Se encontrou a tag que define o começo de um poço
                    if (LeitorHelperTR2.ÉComeçoDePoço(linha))
                    {
                        // Troca o modo de leitura pra standby, pra que o leitor se prepare pra receber dados
                        modoLeitura = ModoLeituraArquivoTR2.Standby;
                    }
                    // Se a linha for qualquer outra tag
                    else
                    {
                        // A linha só deve ser processada se o modo de leitura for qualquer coisa diferente de "nenhum"
                        if (modoLeitura != ModoLeituraArquivoTR2.Nenhum)
                        {
                            // Se a linha contiver o nome do poço
                            if (LeitorHelperTR2.ÉLinhaComNomeDoPoço(linha))
                            {
                                // Extrai o nome do poço da tag
                                poçoAtual = LeitorHelperTR2.ObterValorTag(linha);

                                // Se o nome do poço extraído da tag (que é o poço atualmente sendo lido)
                                if (poçoAtual != _poçoSelecionado)
                                {
                                    _leitorDadosGerais.Reset();
                                    _leitorTrajetória.Reset();
                                    _leitorLitologia.Reset();
                                    _leitorPerfis.Reset();
                                    _leitorSapatas.Reset();
                                    _leitorObjetivos.Reset();
                                    _leitorEstratigrafia.Reset();
                                    modoLeitura = ModoLeituraArquivoTR2.Nenhum;
                                }
                                else
                                {
                                    lerPoço = true;
                                }
                            }
                            else if (LeitorHelperTR2.ÉFimDePoço(linha))
                            {
                                break;
                            }
                            else if (modoLeitura == ModoLeituraArquivoTR2.Standby)
                            {
                                if (LeitorHelperTR2.ÉDataset(linha))
                                {
                                    if (LeitorHelperTR2.ÉLinhaDeLitologia(linha) && LitologiasSelecionadas.Count > 0)
                                    {
                                        modoLeitura = ModoLeituraArquivoTR2.Litologias;
                                    }
                                    else if (LeitorHelperTR2.ÉLinhaDePerfil(linha) && PerfisSelecionados.Count > 0)
                                    {
                                        modoLeitura = ModoLeituraArquivoTR2.Perfis;
                                    }
                                    else if (LeitorHelperTR2.ÉLinhaDeRegistro(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.Registros))
                                    {
                                        modoLeitura = ModoLeituraArquivoTR2.Registros;
                                    }
                                    else if(LeitorHelperTR2.ÉLinhaDeSapata(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.Sapatas))
                                    {
                                        modoLeitura = ModoLeituraArquivoTR2.Sapatas;
                                    }
                                    else if (LeitorHelperTR2.ÉLinhaDeObjetivo(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.Objetivos))
                                    {
                                        modoLeitura = ModoLeituraArquivoTR2.Objetivo;
                                    }
                                    else if (LeitorHelperTR2.ÉLinhaDeEstratigrafia(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.Estratigrafia))
                                    {
                                        modoLeitura = ModoLeituraArquivoTR2.Estratigrafia;
                                    }
                                }
                                else if (LeitorHelperTR2.ÉDadosGerais(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais))
                                {
                                    modoLeitura = ModoLeituraArquivoTR2.DadosGerais;
                                }
                            }
                            else if (LeitorHelperTR2.ÉFimDeDataset(linha))
                            {
                                modoLeitura = ModoLeituraArquivoTR2.Standby;

                                if (LeitorHelperTR2.ÉFimDeLitologia(linha) && LitologiasSelecionadas.Count > 0)
                                {
                                    _leitorLitologia.AdicionarÚltimoPonto();
                                }
                                else if (LeitorHelperTR2.ÉFimDePerfil(linha) && PerfisSelecionados.Count > 0)
                                {
                                    _leitorPerfis.ProcessaLinha(linha);
                                }
                                else if (LeitorHelperTR2.ÉFimDeSapata(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.Sapatas))
                                {
                                    _leitorSapatas.ProcessaLinha(linha);
                                }
                                else if(LeitorHelperTR2.ÉFimDeObjetivo(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.Objetivos))
                                {
                                    _leitorObjetivos.ProcessaLinha(linha);
                                }
                                else if(LeitorHelperTR2.ÉFimDeEstratigrafia(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.Estratigrafia))
                                {
                                    _leitorEstratigrafia.ProcessaLinha(linha);
                                }
                            }
                            else if (LeitorHelperTR2.ÉFimDeDadosGerais(linha))
                            {
                                modoLeitura = ModoLeituraArquivoTR2.Standby;
                            }
                            else if (modoLeitura == ModoLeituraArquivoTR2.DadosGerais)
                            {
                                if (LeitorHelperTR2.ÉLinhaDeTrajetória(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.Trajetória))
                                {
                                    modoLeitura = ModoLeituraArquivoTR2.Trajetória;
                                }
                                else
                                {
                                    _leitorDadosGerais.ProcessaLinha(linha);
                                }
                            }
                            else if (modoLeitura == ModoLeituraArquivoTR2.Trajetória)
                            {
                                _leitorTrajetória.ProcessaLinha(linha);
                            }
                            else if (modoLeitura == ModoLeituraArquivoTR2.Litologias)
                            {
                                _leitorLitologia.ProcessaLinha(linha);
                            }
                            else if (modoLeitura == ModoLeituraArquivoTR2.Perfis)
                            {
                                _leitorPerfis.ProcessaLinha(linha);
                            }
                            else if(modoLeitura == ModoLeituraArquivoTR2.Sapatas)
                            {
                                _leitorSapatas.ProcessaLinha(linha);
                            }
                            else if (modoLeitura == ModoLeituraArquivoTR2.Objetivo)
                            {
                                _leitorObjetivos.ProcessaLinha(linha);
                            }
                            else if (modoLeitura == ModoLeituraArquivoTR2.Estratigrafia)
                            {
                                _leitorEstratigrafia.ProcessaLinha(linha);
                            }
                            else if (modoLeitura == ModoLeituraArquivoTR2.Registros)
                            {
                                if (DadosSelecionados.Contains(DadosSelecionadosEnum.Registros) || DadosSelecionados.Contains(DadosSelecionadosEnum.Eventos))
                                    _leitorRegistros.ProcessaLinha(linha);
                            }
                        }
                    }

                    i += 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
            }
            
        }


        public override bool TryGetRegistros(out List<RegistroDTO> registrosDePerfuração)
        {
            registrosDePerfuração = _leitorRegistros != null && _leitorRegistros.Registros != null ? _leitorRegistros.Registros : null;
            return true;
        }

        public override bool TryGetEventos(out List<RegistroDTO> registrosDePerfuração)
        {
            registrosDePerfuração = null;
            return true;
        }
    }
}
