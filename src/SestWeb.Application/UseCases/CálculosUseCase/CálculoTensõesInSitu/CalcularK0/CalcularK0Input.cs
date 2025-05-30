using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0
{
    public class CalcularK0Input
    {
        public string IdPoço { get; set; }
        public List<LotInput> ListaLot { get; set; } = new List<LotInput>();
        public string PerfilTensãoVerticalId { get; set; }
        public string PerfilGPOROId { get; set; }
        public double? Coeficiente { get; set; }

    }
}
