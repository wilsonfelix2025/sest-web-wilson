using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularNormalizaçãoPP
{
    public class CalcularNormalizaçãoPPRequest
    {
        public string IdPoço { get; set; }
        public string PerfilGPOROId { get; set; }
        public string PerfilTensãoVerticalId { get; set; }
        public List<LotRequest> ListaLot {get; set;}
        public double? Coeficiente { get; set; }

    }
}