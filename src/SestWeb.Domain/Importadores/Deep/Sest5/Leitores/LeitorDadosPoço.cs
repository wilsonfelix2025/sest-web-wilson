using System.Globalization;
using System.Xml.Linq;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;

namespace SestWeb.Domain.Importadores.Deep.Sest5.Leitores
{
    public static class LeitorDadosPoço
    {
        public static DadosGerais ObterDadosPoço(XDocument xDoc)
        {
            var identificaçãoElement = xDoc?.Root?.Element("Identificacao");
            var identificação = ObterIdentificação(identificaçãoElement);

            var geraisElement = xDoc?.Root?.Element("Geral");
            var geometria = ObterGeometria(geraisElement);

            var areaElement = xDoc?.Root?.Element("Area");
            var area = ObterArea(areaElement);

            return new DadosGerais()
            {
                Identificação = identificação,
                Geometria = geometria,
                Area = area
            };
        }

        private static double ConverterParaDouble(string s)
        {
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            return 0;
        }

        private static Area ObterArea(XElement areaElement)
        {
            var densidadeAguaMar = ConverterParaDouble(areaElement.Element("DensidadeAguaMar")?.Value);
            var densidadeSuperficie = ConverterParaDouble(areaElement.Element("DensidadeSuperficie")?.Value);
            var sonicoSuperficie = ConverterParaDouble(areaElement.Element("SonicoSuperficie")?.Value);

            return new Area()
            {
                DensidadeAguaMar = densidadeAguaMar,
                DensidadeSuperficie = densidadeSuperficie,
                SonicoSuperficie = sonicoSuperficie
            };
        }

        private static Geometria ObterGeometria(XElement geometriaElement)
        {
            var antePoço = ConverterParaDouble(geometriaElement.Element("AntePoco")?.Value);
            var mesaRotativa = ConverterParaDouble(geometriaElement.Element("MesaRotativa")?.Value);
            var laminaAgua = ConverterParaDouble(geometriaElement.Element("LaminaAgua")?.Value);
            var xutmBase = ConverterParaDouble(geometriaElement.Element("XUTMBase")?.Value);
            var yutmBase = ConverterParaDouble(geometriaElement.Element("YUTMBase")?.Value);
            var cotaAltimetrica = ConverterParaDouble(geometriaElement.Element("CotaAltimetrica")?.Value);

            var mesaRotativAdaptada = 0.0;

            if (mesaRotativa != 0)
            {
                mesaRotativAdaptada = mesaRotativa;
            }
            else if (antePoço != 0)
            {
                mesaRotativAdaptada = antePoço;
            }

            return new Geometria()
            {
                OffShore = new OffShore()
                {
                    LaminaDagua = laminaAgua
                },
                OnShore = new OnShore()
                {
                    AlturaDeAntePoço = antePoço,
                    Elevação = cotaAltimetrica
                },
                Coordenadas = new Coordenadas()
                {
                    UtMx = xutmBase,
                    UtMy = yutmBase
                },

                MesaRotativa = mesaRotativAdaptada,
            };
        }

        private static Identificação ObterIdentificação(XElement identificaçãoElement)
        {
            if (identificaçãoElement == null)
            {
                return null;
            }

            var nome = identificaçãoElement.Element("Nome")?.Value;
            var tipo = int.Parse(identificaçãoElement.Element("Tipo").Value);
            var bacia = identificaçãoElement.Element("Bacia").Value;
            var campo = identificaçãoElement.Element("Campo").Value;
            var sonda = identificaçãoElement.Element("Sonda").Value;
            var companhia = identificaçãoElement.Element("Companhia").Value;

            return new Identificação
            {
                NomePoço = nome,
                Bacia = bacia,
                Campo = campo,
                Sonda = sonda,
                Companhia = companhia,
                TipoPoço = (TipoPoço)tipo,
                NomePoçoLocalImportação = "SEST"
            };
        }
    }
}