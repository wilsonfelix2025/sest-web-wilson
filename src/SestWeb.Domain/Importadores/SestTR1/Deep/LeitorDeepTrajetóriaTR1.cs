using SestWeb.Domain.DTOs;
using SestWeb.Domain.Importadores.SestTR1.Utils;
using System.Collections.Generic;

namespace SestWeb.Domain.Importadores.SestTR1.Deep
{
    public class LeitorDeepTrajetóriaTR1
    {
        public TrajetóriaDTO Trajetória { get; private set; }
        public LeitorDeepTrajetóriaTR1(List<string> linhas)
        {
            List<PontoTrajetóriaDTO> pontos = new List<PontoTrajetóriaDTO>();

            foreach (var linha in linhas)
            {
                if (LeitorHelperTR1.ÉPontoDeTrajetória(linha))
                {
                    pontos.Add(new PontoTrajetóriaDTO
                    {
                        Pm = LeitorHelperTR1.ObterAtributo(linha, "PM"),
                        Inclinação = LeitorHelperTR1.ObterAtributo(linha, "Inclination"),
                        Azimute = LeitorHelperTR1.ObterAtributo(linha, "Azimuth"),
                    });
                }
            }

            Trajetória = new TrajetóriaDTO { Pontos = pontos };
        }
    }
}
