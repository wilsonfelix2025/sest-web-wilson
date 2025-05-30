using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using System;
using System.Collections.Generic;
using System.IO;

namespace SestWeb.Domain.Importadores.Shallow.SestTR2
{
    public class LeitorShallowSestTR2
    {
        private StreamReader _streamReader;

        private List<string> _litologias = new List<string>();

        private List<RetornoPerfis> _perfis = new List<RetornoPerfis>();
        private string _idLitologia = string.Empty;

        public LeitorShallowSestTR2(string caminho)
        {
            this._streamReader = LeitorHelperTR2.AbrirArquivoSestTR2(caminho);
            LeitorHelperTR2.nomesPerfis = new Dictionary<string, string>();
        }

        public DadosLidos LerDados()
        {
            var lendoLitologia = false;
            var lendoPerfil = false;
            var lendoNome = false;
            var tagNome = "";
            var nomeDataset = "";
            var mnemônicoPerfil = "";
            var poçoAtual = "";
            var i = 0;
            var poços = new Dictionary<string, PoçoImportado>();
            var litologiasPoçoAtual = new List<string>();
            var temSapatas = false;
            var temObjetivos = false;
            var temEstratigrafia = false;
            var temDadosGerais = false;
            var temTrajetória = false;
            var temRegistros = false;

            try
            {
                while (!_streamReader.EndOfStream)
                {
                    i += 1;
                    var linha = _streamReader.ReadLine().Trim();

                    if (LeitorHelperTR2.ÉFimDePoçoArquivo(linha))
                    {
                        break;
                    }

                    if (LeitorHelperTR2.ÉLinhaDeRegistro(linha))
                    {
                        temRegistros = true;
                    }
                    else if (LeitorHelperTR2.ÉLinhaDeSapata(linha))
                    {
                        temSapatas = true;
                    }
                    else if (LeitorHelperTR2.ÉLinhaDeObjetivo(linha))
                    {
                        temObjetivos = true;
                    }
                    else if (LeitorHelperTR2.ÉLinhaDeEstratigrafia(linha))
                    {
                        temEstratigrafia = true;
                    }
                    else if (LeitorHelperTR2.ÉDadosGerais(linha))
                    {
                        temDadosGerais = true;
                    }
                    else if (LeitorHelperTR2.ÉLinhaDeTrajetória(linha))
                    {
                        temTrajetória = true;
                    }

                    if (LeitorHelperTR2.ÉLinhaComNomeDoPoço(linha))
                    {
                        poçoAtual = LeitorHelperTR2.ObterValorTag(linha);
                        poços.Add(poçoAtual, new PoçoImportado());
                    }

                    if (LeitorHelperTR2.ÉFimDePoço(linha))
                    {
                        foreach (var litologia in litologiasPoçoAtual)
                        {
                            poços[poçoAtual].Litologias.Add(litologia);
                            _litologias.Add($"{poçoAtual}:{litologia}");
                        }

                        litologiasPoçoAtual.Clear();
                    }

                    if (LeitorHelperTR2.ÉLinhaTipoPoço(linha))
                    {
                        var tipoPoço = LeitorHelperTR2.ObterValorTag(linha);

                        if (tipoPoço == "CorrelacaoRetroAnalise")
                        {
                            tipoPoço = "Retroanalise";
                        }

                        poços[poçoAtual].Tipo = tipoPoço;
                    }

                    if (LeitorHelperTR2.ÉLinhaDeLitologia(linha) || LeitorHelperTR2.ÉLinhaDePerfil(linha))
                    {
                        if (LeitorHelperTR2.ÉLinhaDeLitologia(linha))
                        {
                            lendoLitologia = true;
                        }
                        else
                        {
                            lendoPerfil = true;
                        }
                    }

                    if (lendoLitologia || lendoPerfil)
                    {
                        if (linha.Contains(":Nome "))
                        {
                            if (!lendoNome && nomeDataset == "")
                            {
                                 lendoNome = true;
                                tagNome += linha;
                            }

                            if (lendoLitologia && linha.Contains("z:Id"))
                            {
                                _idLitologia = LeitorHelperTR2.ObterAtributo(linha, "z:Id");                                
                            }
                            
                            if (lendoLitologia && linha.Contains("i:nil=\"true\""))
                            {
                                var id = LeitorHelperTR2.ObterAtributo(linha, "z:Ref");
                                nomeDataset = LeitorHelperTR2.nomesLitologias.GetValueOrDefault(id);
                                litologiasPoçoAtual.Add(nomeDataset);
                                lendoLitologia = false;
                                lendoNome = false;
                                nomeDataset = "";
                                tagNome = "";
                            }

                        }

                        if (linha.Contains(":Nome>"))
                        {
                            if (lendoNome)
                            {
                                lendoNome = false;

                                if (!linha.Contains(":Nome "))
                                {
                                    tagNome += " " + linha;
                                }
                                
                                nomeDataset = LeitorHelperTR2.ObterValorTag(tagNome);
                                tagNome = "";

                                if (lendoPerfil && linha.Contains("z:Id"))
                                {
                                    var id = LeitorHelperTR2.ObterAtributo(linha, "z:Id");
                                    if (!LeitorHelperTR2.nomesPerfis.ContainsKey(id))
                                        LeitorHelperTR2.nomesPerfis.Add(id, nomeDataset);
                                }

                                
                                if (lendoLitologia)
                                {
                                    if (!string.IsNullOrEmpty(nomeDataset) && !LeitorHelperTR2.nomesLitologias.ContainsKey(_idLitologia))
                                        LeitorHelperTR2.nomesLitologias.Add(_idLitologia, nomeDataset);

                                    litologiasPoçoAtual.Add(nomeDataset);
                                    lendoLitologia = false;
                                    nomeDataset = "";
                                }
                            }
                        }

                        if (lendoNome && !lendoLitologia)
                        {
                            if (linha.Contains("i:nil=\"true\""))
                            {
                                lendoNome = false;
                                tagNome = "";
                                var id = LeitorHelperTR2.ObterAtributo(linha, "z:Ref");
                                nomeDataset = LeitorHelperTR2.nomesPerfis.GetValueOrDefault(id);
                            }
                            else
                                tagNome += linha;
                        }

                        if (lendoPerfil && linha.Contains(":Mnemônico>"))
                        {
                            mnemônicoPerfil = LeitorHelperTR2.ObterValorTag(linha);

                            if (mnemônicoPerfil == "THORmin_THORmax")
                            {
                                mnemônicoPerfil = "RET";
                            }
                        }

                        if (lendoPerfil && mnemônicoPerfil != "" && nomeDataset != "")
                        {
                            var símboloUnidade = "";

                            try
                            {
                                símboloUnidade = GrupoUnidades.GetUnidadePadrão(mnemônicoPerfil).Símbolo;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Não foi possível obter a unidade do perfil com mnemônico {mnemônicoPerfil}");
                            }

                            _perfis.Add(new RetornoPerfis($"{poçoAtual}:{nomeDataset}", mnemônicoPerfil, símboloUnidade));
                            poços[poçoAtual].Perfis.Add(new RetornoPerfis(nomeDataset, mnemônicoPerfil, símboloUnidade));
                            lendoPerfil = false;
                            nomeDataset = "";
                            mnemônicoPerfil = "";
                        }
                    }
                }

                foreach (var litologia in litologiasPoçoAtual)
                {
                    _litologias.Add($"{poçoAtual}:{litologia}");
                    poços[poçoAtual].Litologias.Add(litologia);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }  

            try
            {
                var retorno = new DadosLidos
                {
                    TemSapatas = temSapatas,
                    TemObjetivos = temObjetivos,
                    TemEstratigrafia = temEstratigrafia,
                    TemDadosGerais = temDadosGerais,
                    TemTrajetória = temTrajetória,
                    TemLitologia = _litologias.Count > 0,
                    Litologias = _litologias,
                    Perfis = _perfis,
                    Poços = poços,
                    TemRegistros = temRegistros,
                    TemEventos = true
                };

                Console.WriteLine($"{poços.Count} poços encontrados no arquivo.");

                return retorno;
            }
            catch (Exception e)
            {
                throw new Exception($"LeitorShallowSestTR2 - Não foi possível ler dados - {e.Message}");
            }
        }
    }
}
