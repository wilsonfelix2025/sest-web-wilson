
using System.Collections.Generic;

namespace SestWeb.Domain.DTOs
{
    public class TrendDTO
    {
        public string NomeTrend { get; set; } = string.Empty;
        public List<TrechoTrendDTO> Trechos { get; set; }
    }
}
