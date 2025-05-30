using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Importadores.Shallow.SestTR2;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{
    public class LeitorDeepObjetivosTR2
    {
        private string _objetivoAtual;

        private Dictionary<string, ObjetivoDTO> _objetivos { get; set; }

        private ModoLeituraPerfil _modoLeitura { get; set; }

        public LeitorDeepObjetivosTR2()
        {
            Reset();
        }

        public void ProcessaLinha(string linha)
        {
            if (linha.Contains("<d5p1:Profundidade") && _objetivoAtual == "")
            {
                var pm = LeitorHelperTR2.ObterValorTag(linha);
                _objetivoAtual = pm;
            }
            else if (linha.Contains("<d5p1:Tipo") && _objetivoAtual != "")
            {
                var tipo = LeitorHelperTR2.ObterValorTag(linha);
                var objetivoDTO = new ObjetivoDTO(_objetivoAtual, tipo);
                _objetivos.Add(_objetivoAtual, objetivoDTO);
            }
            else if (LeitorHelperTR2.ÉFimDeObjetivo(linha))
            {
                _objetivoAtual = "";
            }
        }

        public List<ObjetivoDTO> ObterObjetivos()
        {
            return _objetivos.Values.ToList();
        }

        public void Reset()
        {
            _objetivos = new Dictionary<string, ObjetivoDTO>();
            _objetivoAtual = "";
            _modoLeitura = ModoLeituraPerfil.Nenhum;
        }
    }
}
