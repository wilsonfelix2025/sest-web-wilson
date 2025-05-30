using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.PontosEntity;
using System.Collections.Generic;
using SestWeb.Domain.Importadores.Shallow.SestTR2;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{
    public class LeitorDeepPerfisTR2
    {
        public List<PerfilDTO> Perfis { get; private set; } = new List<PerfilDTO>();

        private Dictionary<string, PerfilDTO> _perfis { get; set; }

        private string _tagNome { get; set; }

        private string _perfilAtual;

        private List<string> _perfisSelecionados = new List<string>();

        private PontoDTO _pontoAtual { get; set; }

        private ModoLeituraPerfil _modoLeitura { get; set; }

        public LeitorDeepPerfisTR2(List<string> perfisSelecionados)
        {
            _perfisSelecionados = perfisSelecionados;
            Reset();
        }

        public void ProcessaLinha(string linha)
        {
            if (linha.Contains("<d5p1:Nome") && _perfilAtual == "")
            {
        
                var nomePerfil = LeitorHelperTR2.ObterValorTag(linha);

                if (linha.Contains("i:nil=\"true\""))
                {
                    var id = LeitorHelperTR2.ObterAtributo(linha, "z:Ref");
                    nomePerfil = LeitorHelperTR2.nomesPerfis.GetValueOrDefault(id);
                }

                if (_perfisSelecionados.Contains(nomePerfil))
                {
                    _perfilAtual = nomePerfil;
                    _perfis.Add(_perfilAtual, new PerfilDTO
                    {
                        Nome = nomePerfil,
                        PontosDTO = new List<PontoDTO>(),
                    });
                }
                else
                {
                    _perfilAtual = "";
                }
            }

            if (_perfilAtual != "")
            {
                if (linha.Contains("<d2p1:Key "))
                    _modoLeitura = ModoLeituraPerfil.Pm;
                else if (linha.Contains("<d2p1:Value "))
                    _modoLeitura = ModoLeituraPerfil.Valor;
                else if (linha.Contains("<d8p1:_valor>") && _modoLeitura == ModoLeituraPerfil.Pm)
                {
                    _pontoAtual.Pm = LeitorHelperTR2.ObterValorTag(linha);
                    _modoLeitura = ModoLeituraPerfil.Nenhum;
                }
                else if (linha.Contains("<d8p1:Valor>") && _modoLeitura == ModoLeituraPerfil.Valor)
                {
                    _pontoAtual.Valor = LeitorHelperTR2.ObterValorTag(linha);
                    _modoLeitura = ModoLeituraPerfil.Nenhum;
                }
                else if (linha.Contains("<d5p1:Mnemônico"))
                {
                    var mnemônicoPerfil = LeitorHelperTR2.ObterValorTag(linha);
                    
                    if (mnemônicoPerfil == "THORmin_THORmax")
                    {
                        mnemônicoPerfil = "RET";
                    }

                    _perfis[_perfilAtual].Mnemonico = mnemônicoPerfil;
                }

                if (_pontoAtual.Pm != "" && _pontoAtual.Valor != "")
                {
                    _perfis[_perfilAtual].PontosDTO.Add(_pontoAtual);
                    ResetPontoAtual();
                }
            }

            if (LeitorHelperTR2.ÉFimDePerfil(linha))
            {
                _perfilAtual = "";
            }
        }

        public List<PerfilDTO> ObterPerfis()
        {
            foreach (var entry in _perfis)
            {
                Perfis.Add(entry.Value);
            }

            return Perfis;
        }

        public void Reset()
        {
            _tagNome = "";
            _perfis = new Dictionary<string, PerfilDTO>();
            _perfilAtual = "";
            _modoLeitura = ModoLeituraPerfil.Nenhum;
            ResetPontoAtual();
        }

        private void ResetPontoAtual()
        {
            _pontoAtual = new PontoDTO
            {
                Pm = "",
                Valor = "",
                Origem = OrigemPonto.Importado
            };
        }
    }

    internal enum ModoLeituraPerfil
    {
        Nenhum,
        Pm,
        Valor
    }
}
