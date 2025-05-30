using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Importadores.Deep.Sest5.Leitores
{
    public static class LeitorLitologiaPrevista
    {
        public static Litologia ObterLitologiaPrevista(XDocument xDoc)
        {
            var litologia = xDoc.Root.Elements("Litologia").Elements("Lito")
                             .Select(n =>
                                 new
                                 {
                                     PMTopo = ConversionExtensions.ToDouble(n.Attribute("PMTopo").Value),
                                     PMBase = ConversionExtensions.ToDouble(n.Attribute("PMBase").Value),
                                     TipoRocha = n.Attribute("TipoRocha").Value,
                                 })
                             .ToList();

            if (!litologia.Any())
                return null;

            var listaPontoLitologia = new List<PontoLitologia>();

            //foreach (var item in litologia)
            //{
            //    var pontoLitologia = new PontoLitologia(item.PMTopo, TipoRocha.FromMnemonico(item.TipoRocha));
            //    listaPontoLitologia.Add(pontoLitologia);
            //}

            //var ultimoPonto = litologia.Last();

            //listaPontoLitologia.Add(new PontoLitologia(ultimoPonto.PMBase, TipoRocha.FromMnemonico(ultimoPonto.TipoRocha)));

            //var lito = new Litologia(TipoLitologia.Prevista);
            //lito.Reset(listaPontoLitologia);
            //return lito;

            return null;
        }
    }
}