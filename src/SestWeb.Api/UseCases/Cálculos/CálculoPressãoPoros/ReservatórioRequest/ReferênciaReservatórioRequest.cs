using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPressãoPoros.ReservatórioRequest
{
    public class ReferênciaReservatórioRequest
    {
        public string Poco { get; set; }
        public double Cota { get; set; }
        public double Pp { get; set; }
        public GradienteReservatórioRequest Gradiente { get; set; }
        public ContatosReservatórioRequest Contatos { get; set; }
    }
}
