using System;
using System.Linq;
using System.Xml.Linq;

namespace SestWeb.Domain.Importadores.Shallow.Sest5
{
    public static class LeitorShallowSapatas
    {
        public static bool LerSapatas(XDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc), $"O parâmetro {nameof(xDoc)} não pode ser nulo.");

            var result = xDoc.Root?.Elements("Sapatas").Elements("Sapata").Any();
            return result.HasValue && result.Value;
        }
    }
}