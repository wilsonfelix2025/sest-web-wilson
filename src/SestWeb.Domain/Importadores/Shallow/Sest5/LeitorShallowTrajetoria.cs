using System;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Importadores.Deep.Sest5;

namespace SestWeb.Domain.Importadores.Shallow.Sest5
{
    public static class LeitorShallowTrajetoria
    {
        public static bool LerTrajetoria(XDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc), $"O parâmetro {nameof(xDoc)} não pode ser nulo.");
            
            var tipoPoço = LeitorTipoPoço.ObterTipoPoço(xDoc);
            string tag;
            switch (tipoPoço)
            {
                case TipoPoço.Projeto:
                    tag = "TrajProj";
                    break;

                case TipoPoço.Retroanalise:
                    tag = "TrajRetro";
                    break;

                default:
                    throw new ArgumentException("Tipo poço não reconhecido!");
            }
            
            var result = xDoc.Root?.Element("Trajetoria")?.Elements(tag).Any();
            return result.HasValue && result.Value;
        }
    }
}
