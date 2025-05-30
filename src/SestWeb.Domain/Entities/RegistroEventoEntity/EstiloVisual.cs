namespace SestWeb.Domain.Entities.RegistrosEventos
{
    public class EstiloVisual
    {
        public EstiloVisual()
        {
            Marcador = "Nenhum";
            CorDoMarcador = "Transparent";
            ContornoDoMarcador = "Transparent";
        }

        public EstiloVisual(string marcador, string corDoMarcador, string contornoDoMarcador)
        {
            Marcador = marcador;
            CorDoMarcador = corDoMarcador;
            ContornoDoMarcador = contornoDoMarcador;
        }
        public string Marcador { get; private set; }
        public string CorDoMarcador { get; private set; }
        public string ContornoDoMarcador { get; private set; }
    }
}