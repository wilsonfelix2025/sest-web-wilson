namespace SestWeb.Domain.DTOs
{
    public class DadosGeraisDTO
    {
        public GeometriaDTO Geometria { get; set; } = new GeometriaDTO();
        public IdentificaçãoDTO Identificação { get; set; } = new IdentificaçãoDTO();
        public AreaDTO Area { get; set; } = new AreaDTO();
    }


}
