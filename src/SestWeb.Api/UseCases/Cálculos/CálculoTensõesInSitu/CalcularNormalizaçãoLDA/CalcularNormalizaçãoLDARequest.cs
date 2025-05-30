using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularNormalizaçãoLDA
{
    public class CalcularNormalizaçãoLDARequest
    {
        public string IdPoço { get; set; }
        public double? Coeficiente { get; set; }
        public string PerfilTensãoVerticalId { get; set; }
        public List<LotRequest> ListaLot {get; set;}
    }
}