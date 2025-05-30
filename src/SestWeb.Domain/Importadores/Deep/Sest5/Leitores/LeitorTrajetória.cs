using System;
using System.Xml.Linq;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Importadores.Deep.Sest5.Leitores
{
    public class LeitorTrajetória
    {
        public static Trajetória ObterTrajetória(XDocument xDoc)
        {
            var trajetóriaElement = xDoc?.Root?.Element("Trajetoria");
            var método = (MétodoDeCálculoDaTrajetória)int.Parse(trajetóriaElement.Element("Metodo").Value);

            var trajetória = new Trajetória(método);

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

            foreach (var xElement in trajetóriaElement.Elements(tag))
            {
                trajetória.AddPonto(ConversionExtensions.ToDouble(xElement.Attribute("PM").Value),
                    ConversionExtensions.ToDouble(xElement.Attribute("Inclinacao").Value),
                    ConversionExtensions.ToDouble(xElement.Attribute("Azimute").Value));
            }

            return trajetória;
        }
    }
}