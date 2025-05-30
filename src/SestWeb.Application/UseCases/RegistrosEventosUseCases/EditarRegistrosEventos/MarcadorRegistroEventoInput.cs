
namespace SestWeb.Application.UseCases.RegistrosEventosUseCases.EditarRegistrosEventos
{
    public class MarcadorRegistroEventoInput
    {
        public string IdRegistroEvento { get; set; }
        public string Marcador { get; set; }
        public string CorDoMarcador { get; set; }
        public string ContornoDoMarcador { get; set; }
        public string Unidade { get; set; }
        public double? ValorPadrão { get; set; }
    }
}
