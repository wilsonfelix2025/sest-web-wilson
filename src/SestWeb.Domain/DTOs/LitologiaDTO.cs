using System.Collections.Generic;

namespace SestWeb.Domain.DTOs
{
    public class LitologiaDTO
    {
        public string Classificação { get; set; }
        public string Nome { get; set; }
        public string TipoProfundidade { get; set; }
        public List<PontoLitologiaDTO> Pontos { get; set; } = new List<PontoLitologiaDTO>();
    }
}
