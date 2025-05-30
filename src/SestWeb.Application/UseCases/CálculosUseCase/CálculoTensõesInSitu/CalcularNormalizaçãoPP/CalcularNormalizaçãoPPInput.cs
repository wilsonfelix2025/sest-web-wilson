using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoPP
{
    public class CalcularNormalizaçãoPPInput
    {
        public string IdPoço { get; set; }
        public double? Coeficiente { get; set; }
        public List<LotInput> ListaLot { get; set; } = new List<LotInput>();
        public string PerfilTensãoVerticalId { get; set; }
        public string PerfilGPOROId { get; set; }
    }
}