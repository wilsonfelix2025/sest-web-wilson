using System.Collections.Generic;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarLitologiaUseCase
{
    public class LitologiaInput : ImportarDadosGenericInput
    {
        public string TipoLitologia { get; set; }
        public TipoProfundidade TipoProfundidade { get; set; }
        public List<PontoLitologiaInput> PontosLitologia { get; set; } = new List<PontoLitologiaInput>();
    }
}
