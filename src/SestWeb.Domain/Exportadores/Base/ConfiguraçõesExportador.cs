namespace SestWeb.Domain.Exportadores.Base
{
    public class ConfiguraçõesExportador
    {
        public double Topo { get; set; }
        public double Base { get; set; }
        public double Intervalo { get; set; }
        public bool DeveExportarTrajetoria { get; set; }
        public bool DeveExportarLitologia { get; set; }
        public bool DeveExportarPv { get; set; }
        public bool DeveExportarCota { get; set; }
    }
}
