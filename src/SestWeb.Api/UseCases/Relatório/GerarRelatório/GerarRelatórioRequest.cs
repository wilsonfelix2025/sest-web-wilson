using SestWeb.Api.UseCases.Relatório;
using System.Collections.Generic;

namespace SestWeb.Api.UseCases.GerarRelatório
{
    public class GerarRelatórioRequest
    {
        public bool AlturaMR { get; set; }
        public List<RelatórioEntryRequest> Estratigrafias { get; set; }
        public List<RelatórioEntryRequest> Graficos { get; set; }
        public string IdPoço { get; set; }
        public bool Lda { get; set; }
        public RelatórioEntryRequest Litologia { get; set; }
        public bool Nome { get; set; }
        public bool ProfundidadeFinal { get; set; }
        public bool Tipo { get; set; } 
        public RelatórioEntryRequest Trajetoria { get; set; }
    }
}
