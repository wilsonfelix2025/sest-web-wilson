using System;
using SestWeb.Domain.DTOs;
using System.Collections.Generic;
using SestWeb.Domain.Importadores.Shallow.SestTR2;
using System.Linq;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{
    public class LeitorDeepLitologiaTR2
    {
        public List<LitologiaDTO> Litologias { get; private set; }
        public string ÚltimoPmBaseLido { get; private set; }
        public string ÚltimaRochaLida { get; private set; }
        private Dictionary<string, List<PontoLitologiaDTO>> _litologias { get; set; }

        private string _linhaAnterior = "";

        private string _tagNome = "";

        private string _litologiaAtual = "";

        private List<string> _litologiasSelecionadas = new List<string>();

        private PontoLitologiaDTO _pontoAtual { get; set; }

        public LeitorDeepLitologiaTR2(List<string> litologiasSelecionadas)
        {
            _litologiasSelecionadas = litologiasSelecionadas;
            Reset();
        }

        public void ProcessaLinha(string linha)
        {
            if (linha.Contains("<d5p1:Nome") || linha.Contains("</d5p1:Nome>"))
            {
                if (linha.Contains("i:nil=\"true\""))
                {
                    var id = LeitorHelperTR2.ObterAtributo(linha, "z:Ref");
                    _tagNome = LeitorHelperTR2.nomesLitologias.GetValueOrDefault(id);
                }
                else
                {

                    if (!string.IsNullOrWhiteSpace(_tagNome))
                        _tagNome += " ";

                    _tagNome += linha;
                }
            }

            if (linha.Contains("</d5p1:Nome>") || (linha.Contains("i:nil=\"true\"") && !linha.Contains("trechos")))
            {
                if (_linhaAnterior.Contains("<d5p1:Mnemônico"))
                {
                    _tagNome = "";
                }
                else
                {
                    _tagNome = _tagNome.Replace("\n", " ");
                    var nomeLitologia = string.Empty;

                    if (linha.Contains("i:nil=\"true\""))
                    {
                        nomeLitologia = _tagNome;
                    }
                    else
                    {
                        nomeLitologia = LeitorHelperTR2.ObterValorTag(_tagNome);
                    }

                    if (_litologiasSelecionadas.Contains(nomeLitologia))
                    {
                        _litologiaAtual = nomeLitologia;

                        if (!_litologias.ContainsKey(_litologiaAtual))
                        {
                            _litologias.Add(_litologiaAtual, new List<PontoLitologiaDTO>());
                        }
                    }
                    else
                    {
                        _litologiaAtual = "";
                        _tagNome = "";
                    }
                }
            }

            if (_litologiaAtual != "") 
            {
                if (linha.Contains("<d5p1:PmTopo"))
                    _pontoAtual.Pm = LeitorHelperTR2.ObterValorTag(linha);
                else if(linha.Contains("<d5p1:PmBase"))
                    ÚltimoPmBaseLido = LeitorHelperTR2.ObterValorTag(linha);
                else if (linha.Contains("<d5p1:Código"))
                {
                    _pontoAtual.TipoRocha = LeitorHelperTR2.ObterValorTag(linha);
                    ÚltimaRochaLida = _pontoAtual.TipoRocha;
                }
                if (_pontoAtual.Pm != "" && _pontoAtual.TipoRocha != "")
                {
                    _litologias[_litologiaAtual].Add(_pontoAtual);
                    ResetPontoAtual();
                }
            }


            if (_linhaAnterior.Contains("<d5p1:Mnemônico"))
            {
                _tagNome = "";
            }

            _linhaAnterior = linha;
        }

        public List<LitologiaDTO> ObterLitologias()
        {
            foreach (var entry in _litologias)
            {
                Litologias.Add(new LitologiaDTO
                {
                    Nome = entry.Key,
                    Classificação = "Prevista",
                    Pontos = entry.Value
                });
            }

            return Litologias;
        }

        public void Reset()
        {
            _tagNome = "";
            _litologiaAtual = "";
            _litologias = new Dictionary<string, List<PontoLitologiaDTO>>();
            Litologias = new List<LitologiaDTO>();
            ResetPontoAtual();
        }

        private void ResetPontoAtual()
        {
            _pontoAtual = new PontoLitologiaDTO
            {
                Pm = "",
                TipoRocha = ""
            };
        }

        internal void AdicionarÚltimoPonto()
        {
            if (_litologiaAtual != "")
            {
                _pontoAtual.TipoRocha = ÚltimaRochaLida;
                _pontoAtual.Pm = ÚltimoPmBaseLido;
                _litologias[_litologiaAtual].Add(_pontoAtual);
            }
        }
    }
}
