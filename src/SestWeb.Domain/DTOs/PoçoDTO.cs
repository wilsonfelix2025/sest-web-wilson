using System.Collections.Generic;

namespace SestWeb.Domain.DTOs
{
    public class PoçoDTO
    {
        public DadosGeraisDTO DadosGerais { get; set; } = new DadosGeraisDTO();
        public List<PerfilDTO> Perfis { get; set; } = new List<PerfilDTO>();
        public TrajetóriaDTO Trajetória { get; set; } = new TrajetóriaDTO();
        public List<LitologiaDTO> Litologias { get; set; } = new List<LitologiaDTO>();
        public List<SapataDTO> Sapatas { get; set; } = new List<SapataDTO>();
        public List<ObjetivoDTO> Objetivos { get; set; } = new List<ObjetivoDTO>();
        public EstratigrafiaDTO Estratigrafia { get; set; } = new EstratigrafiaDTO();
        public StateDTO State { get; set; } = new StateDTO();
        public List<RegistroDTO> Registros { get; set; } = new List<RegistroDTO>();
        public List<RegistroDTO> Eventos { get; set; } = new List<RegistroDTO>();

    }
}
