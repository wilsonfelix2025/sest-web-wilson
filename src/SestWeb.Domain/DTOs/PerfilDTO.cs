using System.Collections.Generic;

namespace SestWeb.Domain.DTOs
{
    public class PerfilDTO
    {
        public string Descrição { get; set; }
        public string Grupo { get; set; }
        public string IdPoço { get; set; }
        public string Mnemonico { get; set; }
        public string Nome { get; set; }
        public string TipoProfundidade { get; set; }
        public List<PontoDTO> PontosDTO { get; set; } = new List<PontoDTO>();
        public string Unidade { get; set; }
    }
}
