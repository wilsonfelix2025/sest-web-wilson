using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu
{
    public class BreakoutInput
    {
        public string PerfilUCSId { get; set; }
        public string PerfilANGATId { get; set; }
        public string PerfilGPOROId { get; set; }
        public List<BreakoutValoresInput> TrechosBreakout { get; set; }
        public double Azimuth { get; set; }

    }
}