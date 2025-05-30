using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu
{
    public class FraturasTrechosVerticaisRequest
    {
        public string PerfilRESTRId { get; set; }
        public string PerfilGPOROId { get; set; }
        public double Azimuth { get; set; }
        public List<FraturaTrechosVerticaisValoresRequest> TrechosFratura { get; set; } = new List<FraturaTrechosVerticaisValoresRequest>();
    }
}
