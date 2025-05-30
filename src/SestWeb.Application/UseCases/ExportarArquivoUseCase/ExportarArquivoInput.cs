using System.Collections.Generic;

namespace SestWeb.Application.UseCases.ExportarArquivoUseCase
{
    public class ExportarArquivoInput
    {
        public string IdPoço { get; set; }
        public string[] Perfis { get; set; }
        public double Topo { get; set; }
        public double Base { get; set; }
        public double Intervalo { get; set; }
        public bool Trajetoria { get; set; }
        public bool Litologia { get; set; }
        public bool Pv { get; set; }
        public bool Cota { get; set; }
        public string Arquivo { get; set; }
    }
}
