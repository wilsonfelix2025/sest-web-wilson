using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularK0Acompanhamento
{
    public class CalcularK0AcompanhamentoRequest
    {
        public string IdPoço { get; set; }
        public List<LotRequest> ListaLot { get; set; }
        public string PerfilTensãoVerticalId { get; set; }
        public string PerfilGPOROId { get; set; }
        public double? Coeficiente { get; set; }

    }
}