using System.Collections.Generic;

namespace SestWeb.Domain.DTOs.Cálculo.TensõesInSitu
{
    public class RetornoLotDTO
    {
        public List<RetornoPontoLotDTO> PontosDTO { get; set; } = new List<RetornoPontoLotDTO>();
        public double Coeficiente { get; set; }

    }

    public class RetornoPontoLotDTO
    {
        public double valorX { get; set; }
        public double valorY { get; set; }
    }
}
