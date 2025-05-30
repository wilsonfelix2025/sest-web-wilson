
namespace SestWeb.Application.UseCases.RegistrosEventosUseCases.EditarRegistrosEventos
{
    public class TrechoRegistroEventoInput
    {
        public string IdRegistroEvento { get; set; }
        public double Pm { get; set; }
        public double Pv { get; set; }
        public double PmTopo { get; set; }
        public double PmBase { get; set; }
        public double PvTopo { get; set; }
        public double PvBase { get; set; }
        public double? Valor { get; set; }
        public string Comentário { get; set; }
    }
}
