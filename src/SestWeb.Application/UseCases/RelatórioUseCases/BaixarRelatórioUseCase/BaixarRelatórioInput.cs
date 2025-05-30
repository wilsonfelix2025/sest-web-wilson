using SestWeb.Application.UseCases.GerarRelatórioUseCase;
using SestWeb.Application.UseCases.RelatórioUseCases;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.BaixarRelatórioUseCase
{
    public class BaixarRelatórioInput
    {
        public bool AlturaMR { get; set; }
        public List<RelatórioEntryInput> Estratigrafias { get; set; }
        public List<RelatórioEntryInput> Gráficos { get; set; }
        public string IdPoço { get; set; }
        public bool Lda { get; set; }
        public RelatórioEntryInput Litologia { get; set; }
        public bool Nome { get; set; }
        public bool ProfundidadeFinal { get; set; }
        public bool Tipo { get; set; }
        public RelatórioEntryInput Trajetória { get; set; }
    }
}
