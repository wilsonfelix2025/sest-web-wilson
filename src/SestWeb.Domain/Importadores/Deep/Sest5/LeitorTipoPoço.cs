using System.Xml.Linq;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public static class LeitorTipoPoço
    {
        public static TipoPoço ObterTipoPoço(XDocument xDoc)
        {
            return (TipoPoço)int.Parse(xDoc.Root.Element("Identificacao").Element("Tipo").Value);
        }
    }
}