
namespace SestWeb.Api.UseCases.MontagemPerfis
{
    public class MontarPerfisRequest
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
