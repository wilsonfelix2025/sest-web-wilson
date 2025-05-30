
using System.Collections.Generic;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.DTOs
{
    public class TrajetóriaDTO
    {
        public TipoDeAção? TipoDeAção { get; set; }
        public double? ValorTopo { get; set; }
        public double? ValorBase { get; set; }
        public List<PontoTrajetóriaDTO> Pontos { get; set; } = new List<PontoTrajetóriaDTO>();
    }
}
