using System.Collections.Generic;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroLitologia
{
    public class EditarFiltroLitologiaInput : EditarFiltroInput
    {
        public List<string> LitologiasSelecionadas { get; set; }
    }
}
