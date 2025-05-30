using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Importadores.Deep.Sest5.Leitores
{
    public static class LeitorLitologiaAdaptada
    {
        public static Litologia ObterLitologiaAdaptada(XDocument xDoc)
        {
            var litologia = xDoc.Root.Elements("Perfis").Elements("Perfil")
                               .Select(n =>
                                   new
                                   {
                                       PM = ConversionExtensions.ToDouble(n.Attribute("PM").Value),
                                       LITHO = ConversionExtensions.ToInt(n.Attribute("LITHO").Value),
                                   })
                               .ToList();

            //var pontosLito = litologia.Where(x => x.LITHO >= 0)
            //                          .Select(x => new PontoLitologia(x.PM, TipoRocha.FromNumero(x.LITHO))).ToList();

            //var lito = new Litologia(TipoLitologia.Adaptada);

            //lito.Reset(pontosLito);
            //return lito;

            return null;
        }
    }
}