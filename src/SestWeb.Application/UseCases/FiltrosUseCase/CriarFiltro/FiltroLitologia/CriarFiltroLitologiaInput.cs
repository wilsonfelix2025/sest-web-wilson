using System.Collections.Generic;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroLitologia
{
    public class CriarFiltroLitologiaInput : CriarFiltroInput
    {
        public List<string> LitologiasSelecionadas { get; set; }
    }
}
