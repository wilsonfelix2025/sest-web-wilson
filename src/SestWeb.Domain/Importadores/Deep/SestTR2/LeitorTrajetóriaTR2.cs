using SestWeb.Domain.DTOs;
using System.Collections.Generic;
using SestWeb.Domain.Importadores.Shallow.SestTR2;

namespace SestWeb.Domain.Importadores.Deep.SestTR2
{
    public class LeitorTrajetóriaTR2
    {
        public TrajetóriaDTO Trajetória { get; private set; }
        private PontoTrajetóriaDTO _pontoTemporário { get; set; }

        public LeitorTrajetóriaTR2() 
        {
            Reset();
        }

        public void ProcessaLinha(string linha)
        {
            linha = linha.Trim();

            if (linha.Contains("<d6p1:Pm"))
                _pontoTemporário.Pm = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d6p1:Inclinação"))
                _pontoTemporário.Inclinação = LeitorHelperTR2.ObterValorTag(linha);
            else if (linha.Contains("<d6p1:Azimute"))
                _pontoTemporário.Azimute = LeitorHelperTR2.ObterValorTag(linha);

            if (_pontoTemporário.Pm != "" && _pontoTemporário.Inclinação != "" && _pontoTemporário.Azimute != "")
            {
                Trajetória.Pontos.Add(_pontoTemporário);
                ResetPontoAtual();
            }
        }

        public void Reset()
        {
            Trajetória = new TrajetóriaDTO();
            Trajetória.Pontos = new List<PontoTrajetóriaDTO>();
            ResetPontoAtual();
        }

        private void ResetPontoAtual()
        {
            _pontoTemporário = new PontoTrajetóriaDTO
            {
                Pm = "",
                Inclinação = "",
                Azimute = ""
            };
        }
    }
}
