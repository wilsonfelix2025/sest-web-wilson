using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public static class LeitorLitologiaSest5
    {
        public static LitologiaDTO ObterLitologiaAdaptada(XDocument xDoc)
        {
            PontoLitologiaDTO pontoAnterior = null;
            var litologia = xDoc.Root.Elements("Perfis").Elements("Perfil")
                .Select(n =>
                    new
                    {
                        PM = n.Attribute("PM")?.Value,
                        LITHO = n.Attribute("LITHO")?.Value
                    })
                .ToList();

            var pontosLito = litologia.Where(x => x.LITHO != "0")
                .Select(x => new PontoLitologiaDTO {Pm = x.PM, TipoRocha = TipoRocha.FromNumero(x.LITHO.ToInt()).Mnemonico}).ToList();

            var lito = new LitologiaDTO {Classificação = "Litologia", Pontos = pontosLito, Nome = "Litologia"};

            return lito;
        }
    }
}