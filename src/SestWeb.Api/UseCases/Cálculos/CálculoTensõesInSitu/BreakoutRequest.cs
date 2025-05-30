using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu
{
    public class BreakoutRequest
    {
        public string PerfilUCSId { get; set; }
        public string PerfilANGATId { get; set; }
        public string PerfilGPOROId { get; set; }
        public double Azimuth { get; set; }
        public List<BreakoutValoresRequest> TrechosBreakout { get; set; }
    }
}
