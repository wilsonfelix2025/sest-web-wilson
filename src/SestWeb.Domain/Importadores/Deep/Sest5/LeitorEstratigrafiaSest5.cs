using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public static class LeitorEstratigrafiaSest5
    {
        public static EstratigrafiaDTO ObterEstratigrafia(XDocument xDoc)
        {
            var estratigrafia = new EstratigrafiaDTO();

            if (xDoc.Root != null)
            {
                var itensEstratigrafia =
                    xDoc.Root.Elements("EspecieGeologicaPoco").Elements("EspGeolPoco")
                        .Select(n =>
                            new
                            {
                                Tipo = n.Attribute("Tipo")?.Value,
                                PM = n.Attribute("PM")?.Value,
                                Sigla = n.Attribute("Sigla")?.Value,
                                Descrição = n.Attribute("Descricao")?.Value,
                                Idade = n.Attribute("Idade")?.Value
                            }).ToList();

                foreach (var item in itensEstratigrafia)
                {
                    estratigrafia.Adicionar(item.Tipo, item.PM, item.Sigla, item.Descrição, item.Sigla);
                }
            }

            return estratigrafia;
        }
    }
}
