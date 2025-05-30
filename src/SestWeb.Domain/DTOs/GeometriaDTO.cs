
namespace SestWeb.Domain.DTOs
{
    public class GeometriaDTO
    {
        public OnShoreDTO OnShore { get; set; } = new OnShoreDTO();
        public OffShoreDTO OffShore { get; set; } = new OffShoreDTO();
        public CoordenadasDTO Coordenadas { get; set; } = new CoordenadasDTO();

        public string MesaRotativa { get; set; } = "0";
        public bool AtualizaMesaRotativaComElevação { get; set; } = true;
    }
}
