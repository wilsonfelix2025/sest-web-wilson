using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu
{
    public class FraturasTrechosVerticaisInput
    {
        public string PerfilRESTRId { get; set; }
        public string PerfilGPOROId { get; set; }
        public double Azimuth { get; set; }
        public List<FraturaTrechosVerticaisValoresInput> TrechosFratura { get; set; }
    }
}