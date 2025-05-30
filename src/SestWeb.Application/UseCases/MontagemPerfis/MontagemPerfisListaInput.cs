
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.MontagemPerfis
{
    public class MontagemPerfisListaInput
    {
        public string IdPoçoApoio { get; set; }
        public double PvTopoApoio { get; set; }
        public double PvBaseApoio { get; set; }
        public double PvTopo { get; set; }
        public double PvBase { get; set; }
        public string IdRhobApoio { get; set; }
        public string IdDTCApoio { get; set; }
        public string IdDTSApoio { get; set; }
        public string IdGRayApoio { get; set; }
        public string IdResistApoio { get; set; }
        public string IdNPhiApoio { get; set; }
    }
}
