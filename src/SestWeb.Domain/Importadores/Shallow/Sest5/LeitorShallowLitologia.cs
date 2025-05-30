using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Importadores.Shallow.Sest5
{
    public static class LeitorShallowLitologia
    {
        public static IEnumerable<string> LerLitologia(XDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc), $"O parâmetro {nameof(xDoc)} não pode ser nulo.");

            var litoligias = new List<string>();

            var temLitologia = xDoc.Root?.Elements("Perfis").Elements("Perfil").Any(n => n.Attribute("LITHO")?.Value.ToInt() > 0);
            if (temLitologia.HasValue && temLitologia.Value)
            {
                litoligias.Add("Litologia");
            }

            var temLitologiaPrevista = xDoc.Root?.Elements("Litologia").Elements("Lito").Any();
            if (temLitologiaPrevista.HasValue && temLitologiaPrevista.Value)
            {
                litoligias.Add("Prevista");
            }

            return litoligias;
        }
    }
}
