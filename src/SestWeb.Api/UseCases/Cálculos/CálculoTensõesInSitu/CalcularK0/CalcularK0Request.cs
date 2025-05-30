using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularK0
{
    public class CalcularK0Request
    {
        public string IdPoço { get; set; }
        public List<LotRequest> ListaLot { get; set; }
        public string PerfilTensãoVerticalId { get; set; }
        public string PerfilGPOROId { get; set; }
        public double? Coeficiente { get; set; }

    }
}