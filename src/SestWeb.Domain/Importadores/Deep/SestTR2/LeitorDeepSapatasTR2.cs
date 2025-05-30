using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Importadores.Shallow.SestTR2;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{

    public class LeitorDeepSapatasTR2
    {
        private string _sapataAtual;

        private Dictionary<string, SapataDTO> _sapatas { get; set; }

        private ModoLeituraPerfil _modoLeitura { get; set; }

        public LeitorDeepSapatasTR2()
        {
            Reset();
        }

        public void ProcessaLinha(string linha)
        {
            if (linha.Contains("<d5p1:Diâmetro") && _sapataAtual == "")
            {
                var diâmetro = LeitorHelperTR2.ObterValorTag(linha);
                _sapataAtual = diâmetro;
            }

            if (_sapataAtual != "")
            {
                if (linha.Contains("<d5p1:Profundidade"))
                {
                    var pm = LeitorHelperTR2.ObterValorTag(linha);
                    var sapataDTO = new SapataDTO(pm, _sapataAtual);
                    _sapatas.Add(_sapataAtual, sapataDTO);
                }
                else if (linha.Contains("<d5p1:TipoProfundidade"))
                {
                    // se for pv, converter de pv para pm a profundidade lida.
                }
            }

            if (LeitorHelperTR2.ÉFimDeSapata(linha))
            {
                _sapataAtual = "";
            }
        }

        public List<SapataDTO> ObterSapatas()
        {
            return _sapatas.Values.ToList();
        }

        public void Reset()
        {
            _sapatas = new Dictionary<string, SapataDTO>();
            _sapataAtual = "";
            _modoLeitura = ModoLeituraPerfil.Nenhum;
        }
    }
}
