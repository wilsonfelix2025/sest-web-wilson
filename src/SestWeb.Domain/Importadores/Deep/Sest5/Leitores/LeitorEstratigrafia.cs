using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Importadores.Deep.Sest5.Leitores
{
    public static class LeitorEstratigrafia
    {
        public static Estratigrafia ObterEstratigrafia(Trajetória trajetória , XDocument xDoc)
        {
            var estratigrafia = new Estratigrafia();

            xDoc.Root.Elements("EspecieGeologicaPoco").Elements("EspGeolPoco").Select(n =>
                estratigrafia.CriarItemEstratigrafia(trajetória, n.Attribute("Tipo").Value, new Profundidade(ConversionExtensions.ToDouble(n.Attribute("PM").Value)),
                    n.Attribute("Sigla").Value, n.Attribute("Descricao").Value, n.Attribute("Idade").Value)).ToList();

            return estratigrafia;
        }
    }
}