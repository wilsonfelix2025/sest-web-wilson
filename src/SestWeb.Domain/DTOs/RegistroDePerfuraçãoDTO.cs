using System.Collections.Generic;

namespace SestWeb.Domain.DTOs
{
    public class RegistroDTO
    {
        public List<PontoRegistroDTO> Pontos { get; set; } = new List<PontoRegistroDTO>();
        public string Unidade { get; set; }
        public string Tipo { get; set; }

    }
}
