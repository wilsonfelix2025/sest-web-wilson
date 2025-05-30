using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Deep.Sest5.Leitores
{
    public static class LeitorSapatas
    {
        public static List<SapataDTO> ObterSapatas(XDocument xDoc)
        {
            var sapatasLidas =
                xDoc.Root.Elements("Sapatas").Elements("Sapata")
                    .Select(n =>
                        new
                        {
                            PM = n.Attribute("PM").Value,
                            Diametro = n.Attribute("Diametro").Value
                        })
                    .ToList();

            var sapatas = new List<SapataDTO>();

            foreach (var sapatasLida in sapatasLidas)
            {
                var sapata = new SapataDTO(sapatasLida.PM, sapatasLida.Diametro);
                sapatas.Add(sapata);
            }

            return sapatas;
        }
    }
}