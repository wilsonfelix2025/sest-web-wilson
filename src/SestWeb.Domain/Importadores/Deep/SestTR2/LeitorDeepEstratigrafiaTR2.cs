using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Importadores.Shallow.SestTR2;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{
    public class LeitorDeepEstratigrafiaTR2
    {
        private ItemEstratigrafiaDTO _ItemEstratigrafiaAtual;
        private EstratigrafiaDTO _estratigrafiaDTO;

        public LeitorDeepEstratigrafiaTR2()
        {
            Reset();
        }

        public void ProcessaLinha(string linha)
        {
            if (linha.Contains("<d5p1:Profundidade") && _ItemEstratigrafiaAtual == null)
            {
                var pm = LeitorHelperTR2.ObterValorTag(linha);
                _ItemEstratigrafiaAtual = new ItemEstratigrafiaDTO(pm, string.Empty, string.Empty, string.Empty, String.Empty);
            }
            else if (linha.Contains("<d5p1:Sigla") && _ItemEstratigrafiaAtual != null)
            {
                _ItemEstratigrafiaAtual.Sigla = LeitorHelperTR2.ObterValorTag(linha);
                
            }
            else if(linha.Contains("<d5p1:Tipo") && !linha.Contains("<d5p1:TipoProfundidade") && _ItemEstratigrafiaAtual != null)
            {
                _ItemEstratigrafiaAtual.Idade = LeitorHelperTR2.ObterValorTag(linha);
            }
            else if (linha.Contains("<d5p1:TipoProfundidade") && _ItemEstratigrafiaAtual != null)
            {
                // se for pv, converter para pv o valor lido em pm.
            }
            else if (LeitorHelperTR2.ÉFimDeEstratigrafia(linha))
            {
                _estratigrafiaDTO.Adicionar(_ItemEstratigrafiaAtual.Idade, _ItemEstratigrafiaAtual.Pm,
                    _ItemEstratigrafiaAtual.Sigla, _ItemEstratigrafiaAtual.Descrição, _ItemEstratigrafiaAtual.Idade);
                _ItemEstratigrafiaAtual = null;
            }
        }

        public EstratigrafiaDTO ObterEstratigrafia()
        {
            return _estratigrafiaDTO;
        }

        public void Reset()
        {
            _estratigrafiaDTO = new EstratigrafiaDTO();
            _ItemEstratigrafiaAtual = null;
        }
    }
}
