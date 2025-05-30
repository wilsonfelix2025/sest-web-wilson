using System;
using System.Linq;
using System.Xml.Linq;

namespace SestWeb.Domain.Importadores.Shallow.Sest5
{
    public static class LeitorShallowRegistrosSest5
    {
        public static bool LerRegistros(XDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc), $"O parâmetro {nameof(xDoc)} não pode ser nulo.");

            var result = xDoc.Root?.Elements("RegistroPressaoPoros").Elements("RegPresPoros").Any();

            if (result == false)
                result = xDoc.Root?.Elements("TesteAbsorcao").Elements("TesteAbs").Any();           

            return result.HasValue && result.Value;
        }
       

        public static bool LerEventos(XDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc), $"O parâmetro {nameof(xDoc)} não pode ser nulo.");

            var result = xDoc.Root?.Elements("EventosDePerfuracao").Any();
            return result.HasValue && result.Value;
        }
    }
}
