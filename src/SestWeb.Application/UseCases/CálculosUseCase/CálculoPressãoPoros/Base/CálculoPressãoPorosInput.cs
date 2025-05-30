using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.ReservatórioInput;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.Base
{
    public class CálculoPressãoPorosInput
    {
        public string Nome { get; set; }
        public string IdPoço { get; set; }
        public string Tipo { get; set; }
        public string Gn { get; set; } = "";
        public string Exp { get; set; } = "";
        public string IdPerfilFiltrado { get; set; } = "";
        public string IdGradienteSobrecarga { get; set; } = "";
        public bool CalculoReservatorio { get; set; } = false;
        public string Gpph { get; set; } = "";
        public string IdPph { get; set; } = "";
        public DadosReservatórioInput Reservatório { get; set; } = null;
    }
}
