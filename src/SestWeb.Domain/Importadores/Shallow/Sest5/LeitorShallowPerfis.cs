using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Importadores.Shallow.Sest5
{
    public static class LeitorShallowPerfis
    {
        public static IEnumerable<string> LerPerfis(XDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc), $"O parâmetro {nameof(xDoc)} não pode ser nulo.");

            var dict = new Dictionary<string, List<double>>
            {
                // perfis
                {"DTC", new List<double>()},
                {"DTS", new List<double>()},
                {"DTMC", new List<double>()},
                {"DTMS", new List<double>()},
                {"DENF", new List<double>()},
                {"DENG", new List<double>()},
                {"VCL", new List<double>()},
                {"GRAY", new List<double>()},
                {"REST", new List<double>()},
                {"CALIP", new List<double>()},
                {"PORO", new List<double>()},
                // propriedades mecânicas
                {"YOUNG", new List<double>()},
                {"POISS", new List<double>()},
                {"BIOT", new List<double>()},
                {"RESTR", new List<double>()},
                {"ANGAT", new List<double>()},
                {"COESA", new List<double>()},
                {"PERM", new List<double>()},
                {"KS", new List<double>()},
                {"RESCMP", new List<double>()},
                // tensões
                {"THORm", new List<double>()},
                {"THORM", new List<double>()},
                {"AZTHM", new List<double>()},
                // gradientes
                {"GSOBR", new List<double>()},
                {"GPORO", new List<double>()},
                {"GFRAT", new List<double>()},
                {"GCOLS", new List<double>()},
                {"GCOLI", new List<double>()},
                {"GLAMA", new List<double>()},
                {"GECD", new List<double>()},
                // diâmetro de broca
                {"DIAM_BROCA", new List<double>() }
            };

            LerPerfis(xDoc, dict);
            LerPropriedadesMecânicas(xDoc, dict);
            LerTensões(xDoc, dict);
            LerGradientes(xDoc, dict);
            LerRegistroBroca(xDoc, dict);

            foreach (var (key, value) in dict)
                if (value.Any(v => v > 0) || key.Equals("AZTHM"))
                    yield return key;
        }

        private static void LerPerfis(XDocument xDoc, Dictionary<string, List<double>> dict)
        {
            xDoc.Root?.Elements("Perfis").Elements("Perfil").ToList().ForEach(n =>
            {
                // perfis
                dict["DTC"].Add((double) n.Attribute("DTC"));
                dict["DTS"].Add((double) n.Attribute("DTS"));
                dict["DTMC"].Add((double) n.Attribute("DTMC"));
                dict["DTMS"].Add((double) n.Attribute("DTMS"));
                dict["DENF"].Add((double) n.Attribute("DENF"));
                dict["DENG"].Add((double) n.Attribute("DENG"));
                dict["VCL"].Add((double) n.Attribute("VCL"));
                dict["GRAY"].Add((double) n.Attribute("GRAY"));
                dict["REST"].Add((double) n.Attribute("REST"));
                dict["CALIP"].Add((double) n.Attribute("CALIP"));
                dict["PORO"].Add((double) n.Attribute("PORO"));
            });
        }

        private static void LerPropriedadesMecânicas(XDocument xDoc, Dictionary<string, List<double>> dict)
        {
            xDoc.Root?.Elements("PropriedadeMecanica").Elements("PropMec").ToList().ForEach(n =>
            {
                dict["YOUNG"].Add((double)n.Attribute("YOUNG"));
                dict["POISS"].Add((double)n.Attribute("POISS"));
                dict["BIOT"].Add((double)n.Attribute("BIOT"));
                dict["RESTR"].Add((double)n.Attribute("RESTR"));
                dict["ANGAT"].Add((double)n.Attribute("ANGAT"));
                dict["COESA"].Add((double)n.Attribute("COESA"));
                dict["PERM"].Add((double)n.Attribute("PERM"));
                dict["KS"].Add((double)n.Attribute("KS"));
                dict["RESCMP"].Add((double)n.Attribute("RESCMP"));
            });
        }

        private static void LerTensões(XDocument xDoc, Dictionary<string, List<double>> dict)
        {
            xDoc.Root?.Elements("TensoesInSitu").Elements("TensInSitu").ToList().ForEach(n =>
            {
                dict["THORm"].Add((double)n.Attribute("THORm"));
                dict["THORM"].Add((double)n.Attribute("THORM"));
                dict["AZTHM"].Add((double)n.Attribute("AZTHM"));
            });
        }

        private static void LerGradientes(XDocument xDoc, Dictionary<string, List<double>> dict)
        {
            xDoc.Root?.Elements("Gradientes").Elements("Grad").ToList().ForEach(n =>
            {
                dict["GSOBR"].Add((double)n.Attribute("GSOBR"));
                dict["GPORO"].Add((double)n.Attribute("GPORO"));
                dict["GFRAT"].Add((double)n.Attribute("GFRAT"));
                dict["GCOLS"].Add((double)n.Attribute("GCOLS"));
                dict["GCOLI"].Add((double)n.Attribute("GCOLI"));
                dict["GLAMA"].Add((double)n.Attribute("GLAMA"));
                dict["GECD"].Add((double)n.Attribute("GECD"));
            });
        }

        private static void LerRegistroBroca(XDocument xDoc, Dictionary<string, List<double>> dict)
        {
            xDoc.Root?.Elements("RegistroBroca").Elements("RegBroca").ToList().ForEach(n =>
            {
                dict["DIAM_BROCA"].Add(new FractionalNumber(n.Attribute("Diametro")?.Value).DoubleResult);
            });
        }
    }
}