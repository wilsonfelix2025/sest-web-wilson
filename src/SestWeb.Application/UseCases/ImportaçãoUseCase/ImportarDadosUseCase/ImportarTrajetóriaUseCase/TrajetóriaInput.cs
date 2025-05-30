
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarTrajetóriaUseCase
{
    public class TrajetóriaInput : ImportarDadosGenericInput
    {
        public List<PontoTrajetóriaInput> PontoTrajetória { get; set; } = new List<PontoTrajetóriaInput>();
    }
}
