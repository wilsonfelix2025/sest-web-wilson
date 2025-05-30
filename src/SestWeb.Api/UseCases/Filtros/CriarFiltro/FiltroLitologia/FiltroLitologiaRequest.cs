using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Filtros.CriarFiltro.FiltroLitologia
{
    public class FiltroLitologiaRequest : CriarFiltroRequest
    {
        public List<string> LitologiasSelecionadas { get; set; }
    }
}
