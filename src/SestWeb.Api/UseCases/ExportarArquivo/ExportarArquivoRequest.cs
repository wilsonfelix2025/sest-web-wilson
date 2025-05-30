using System.Collections.Generic;

namespace SestWeb.Api.UseCases.ExportarArquivo
{
    public class ExportarArquivoRequest
    {
        public string IdPoço { get; set; }
        public string[] Perfis { get; set; }
        public double PmTopo { get; set; }
        public double PmBase { get; set; }
        public double Intervalo { get; set; }
        public bool Trajetoria { get; set; }
        public bool Litologia { get; set; }
        public bool Pv { get; set; }
        public bool Cota { get; set; }
        public string Arquivo { get; set; }
    }
}
