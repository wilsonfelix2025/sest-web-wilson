using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Trend.EditarTrend
{
    public class EditarTrendRequest
    {
        public List<EditarTrechosItem> Trechos { get; set; }
        public string IdPerfil { get; set; }
        public string NomeTrend { get; set; }
    }
}
