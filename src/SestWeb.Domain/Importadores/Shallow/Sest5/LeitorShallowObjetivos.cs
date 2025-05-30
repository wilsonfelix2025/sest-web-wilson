using System;
using System.Linq;
using System.Xml.Linq;

namespace SestWeb.Domain.Importadores.Shallow.Sest5
{
    public static class LeitorShallowObjetivos
    {
        public static bool LerObjetivos(XDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc), $"O parâmetro {nameof(xDoc)} não pode ser nulo.");

            var result = xDoc.Root?.Elements("Objetivo").Elements("Obj").Any();
            return result.HasValue && result.Value;
        }
    }
}
