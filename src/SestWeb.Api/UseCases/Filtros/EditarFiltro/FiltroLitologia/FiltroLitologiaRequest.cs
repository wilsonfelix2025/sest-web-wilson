using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Filtros.EditarFiltro.FiltroLitologia
{
    public class FiltroLitologiaRequest : EditarFiltroRequest
    {
        public List<string> LitologiasSelecionadas { get; set; }
    }
}
