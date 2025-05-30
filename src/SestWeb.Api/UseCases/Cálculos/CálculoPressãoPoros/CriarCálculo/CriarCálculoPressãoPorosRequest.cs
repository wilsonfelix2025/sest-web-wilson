namespace SestWeb.Api.UseCases.Cálculos.CálculoPressãoPoros.CriarCálculo
{
    public class CriarCálculoPressãoPorosRequest
    {
        public string Nome { get; set; }
        public string IdPoço { get; set; }
        public string Tipo { get; set; }
        public string Gn { get; set; } = "";
        public string Exp { get; set; } = "";
        public string IdPerfilFiltrado { get; set; } = "";
        public string IdGradienteSobrecarga { get; set; } = "";
        public bool CalculoReservatorio { get; set; } = false;
        public ReservatórioRequest.ReservatórioRequest Reservatorio { get; set; }
        public string Gpph { get; set; } = "";
        public string IdPph { get; set; } = "";
    }
}
