using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Importadores.SestTR1.Utils;
using System.Collections.Generic;

namespace SestWeb.Domain.Importadores.SestTR1.Deep
{
    public class LeitorDeepLitologiaTR1
    {
        public List<LitologiaDTO> Litologias { get; private set; } = new List<LitologiaDTO>();

        public void AdicionarLitologia(List<string> linhas)
        {
            List<PontoLitologiaDTO> pontos = new List<PontoLitologiaDTO>();
            var nomeLitologia = "";

            foreach (var linha in linhas)
            {
                if (LeitorHelperTR1.ÉDataset(linha))
                {
                    nomeLitologia = LeitorHelperTR1.ObterAtributo(linha, "Name");
                }
                else if (LeitorHelperTR1.ÉPontoGenérico(linha))
                {
                    var codigoLitologia = LeitorHelperTR1.ObterAtributo(linha, "Value");
                    var éCódigoVálido = double.TryParse(codigoLitologia, out var _);

                    if (éCódigoVálido)
                    {
                        var tipoRocha = TipoRocha.FromNumero(int.Parse(codigoLitologia));

                        if (tipoRocha != null)
                        {
                            pontos.Add(new PontoLitologiaDTO
                            {
                                Pm = LeitorHelperTR1.ObterAtributo(linha, "Depth"),
                                TipoRocha = codigoLitologia,
                                TipoRochaMnemonico = TipoRocha.FromNumero(int.Parse(codigoLitologia)).Mnemonico
                            });
                        }
                    }                    
                }
            }

            Litologias.Add(new LitologiaDTO
            {
                Classificação = "Litologia SEST TR1",
                Nome = nomeLitologia,
                Pontos = pontos
            });
        }
    }
}
