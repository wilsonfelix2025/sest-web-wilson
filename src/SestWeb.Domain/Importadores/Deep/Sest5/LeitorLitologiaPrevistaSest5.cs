using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public static class LeitorLitologiaPrevistaSest5
    {
        public static LitologiaDTO ObterLitologiaPrevista(XDocument xDoc)
        {
            var litologia = xDoc.Root?.Elements("Litologia")?.Elements("Lito")?
                .Select(n =>
                    new
                    {
                        PMTopo = n.Attribute("PMTopo").Value,
                        PMBase = n.Attribute("PMBase").Value,
                        TipoRocha = n.Attribute("TipoRocha").Value
                    })
                .ToList();

            if (litologia == null)
                return null;

            if (!litologia.Any())
                return null;

            var listaPontoLitologia = new List<PontoLitologiaDTO>();

            foreach (var item in litologia)
            {
                var pontoLitologia = new PontoLitologiaDTO {Pm = item.PMTopo, TipoRocha = TipoRocha.FromMnemonico(item.TipoRocha).Mnemonico};
                listaPontoLitologia.Add(pontoLitologia);
            }

            var lito = new LitologiaDTO {Classificação = "Prevista", Pontos = listaPontoLitologia, Nome = "Prevista"};

            return lito;
        }
    }
}