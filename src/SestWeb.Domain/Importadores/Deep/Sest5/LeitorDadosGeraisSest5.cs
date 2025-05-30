using System.Xml.Linq;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public static class LeitorDadosGeraisSest5
    {
        public static DadosGeraisDTO ObterDadosGerais(XDocument xDoc)
        {
            var dadosGerais = new DadosGeraisDTO();

            var identificaçãoElement = xDoc?.Root?.Element("Identificacao");
            var identificação = ObterIdentificação(identificaçãoElement);

            var geraisElement = xDoc?.Root?.Element("Geral");
            var geometria = ObterGeometria(geraisElement);

            var areaElement = xDoc?.Root?.Element("Area");
            var area = ObterArea(areaElement);

            dadosGerais.Identificação = identificação;
            dadosGerais.Geometria = geometria;
            dadosGerais.Area = area;

            return dadosGerais;
        }

        private static IdentificaçãoDTO ObterIdentificação(XElement identificaçãoElement)
        {
            if (identificaçãoElement == null)
            {
                return null;
            }

            var nome = identificaçãoElement.Element("Nome")?.Value;
            var tipo = identificaçãoElement.Element("Tipo")?.Value;
            var bacia = identificaçãoElement.Element("Bacia")?.Value;
            var campo = identificaçãoElement.Element("Campo")?.Value;
            var sonda = identificaçãoElement.Element("Sonda")?.Value;
            var companhia = identificaçãoElement.Element("Companhia")?.Value;

            return new IdentificaçãoDTO
            {
                NomePoço = string.IsNullOrWhiteSpace(nome) ? "Sem nome" : nome,
                Bacia = bacia,
                Campo = campo,
                Sonda = sonda,
                Companhia = companhia,
                NomePoçoLocalImportação = "SEST5"
               // TipoPoço = tipo
            };
        }

        private static GeometriaDTO ObterGeometria(XElement geometriaElement)
        {
            var antePoço = geometriaElement.Element("AntePoco")?.Value;
            var mesaRotativa = geometriaElement.Element("MesaRotativa")?.Value;
            var laminaAgua = geometriaElement.Element("LaminaAgua")?.Value;
            var xutmBase = geometriaElement.Element("XUTMBase")?.Value;
            var yutmBase = geometriaElement.Element("YUTMBase")?.Value;
            var cotaAltimetrica = geometriaElement.Element("CotaAltimetrica")?.Value;

            return new GeometriaDTO
            {
                OffShore = new OffShoreDTO
                {
                    LaminaDagua = laminaAgua
                },
                OnShore = new OnShoreDTO
                {
                    AlturaDeAntePoço = antePoço,
                    Elevação = cotaAltimetrica
                },
                Coordenadas = new CoordenadasDTO
                {
                    UtMx = xutmBase,
                    UtMy = yutmBase
                },

                MesaRotativa = mesaRotativa,
                AtualizaMesaRotativaComElevação = false
            };
        }

        private static AreaDTO ObterArea(XElement areaElement)
        {
            var densidadeAguaMar = areaElement.Element("DensidadeAguaMar")?.Value;
            var densidadeSuperficie = areaElement.Element("DensidadeSuperficie")?.Value;
            var sonicoSuperficie = areaElement.Element("SonicoSuperficie")?.Value;

            return new AreaDTO
            {
                DensidadeAguaMar = densidadeAguaMar,
                DensidadeSuperficie = densidadeSuperficie,
                SonicoSuperficie = sonicoSuperficie
            };
        }
    }
}
