using SestWeb.Api.UseCases.Cálculos.CálculoPressãoPoros.ReservatórioRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPressãoPoros.ReservatórioRequest
{
    public class ReservatórioRequest
    {
        public string Nome { get; set; }
        public ReferênciaReservatórioRequest Referencia { get; set; }
        public InteresseReservatórioRequest Interesse { get; set; }
    }
}
