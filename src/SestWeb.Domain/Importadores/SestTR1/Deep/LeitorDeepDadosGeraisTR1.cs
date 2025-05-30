using SestWeb.Domain.DTOs;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SestWeb.Domain.Importadores.SestTR1.Deep
{
    public class LeitorDeepDadosGeraisTR1
    {
        public DadosGeraisDTO DadosGerais { get; private set; }

        public LeitorDeepDadosGeraisTR1(List<string> linhas)
        {
            XDocument dadosGeraisXml = XDocument.Parse(string.Join(' ', linhas));

            IdentificaçãoDTO identificação = new IdentificaçãoDTO
            {
                NomePoço = dadosGeraisXml.Root.Element("WellName")?.Value,
                Bacia = dadosGeraisXml.Root.Element("Basin")?.Value,
                Campo = dadosGeraisXml.Root.Element("Field")?.Value,
                NomePoçoLocalImportação = "SEST"
            };

            OnShoreDTO onShore = new OnShoreDTO
            {
                AlturaDeAntePoço = dadosGeraisXml.Root.Element("AlturaDoAntePoço")?.Value,
                Elevação = dadosGeraisXml.Root.Element("AltitudeDoTerreno")?.Value,
                LençolFreático = dadosGeraisXml.Root.Element("LencolFreatico")?.Value
            };

            OffShoreDTO offShore = new OffShoreDTO
            {
                LaminaDagua = dadosGeraisXml.Root.Element("WaterDepth")?.Value
            };

            GeometriaDTO geometria = new GeometriaDTO
            {
                OnShore = onShore,
                OffShore = offShore,
                MesaRotativa = dadosGeraisXml.Root.Element("RotaryTable")?.Value,
            };

            AreaDTO area = new AreaDTO
            {
                DensidadeAguaMar = dadosGeraisXml.Root.Element("SeawaterDensity")?.Value,
                DensidadeSuperficie = dadosGeraisXml.Root.Element("MudLineDensity")?.Value,
                SonicoSuperficie = dadosGeraisXml.Root.Element("MudLineSonic")?.Value
            };

            DadosGerais = new DadosGeraisDTO
            {
                Area = area,
                Geometria = geometria,
                Identificação = identificação
            };
        }
    }
}
