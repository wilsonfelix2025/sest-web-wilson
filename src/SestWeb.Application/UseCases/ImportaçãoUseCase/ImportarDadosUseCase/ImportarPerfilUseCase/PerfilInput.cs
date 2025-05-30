using System.Collections.Generic;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarPerfilUseCase
{
    public class PerfilInput : ImportarDadosGenericInput
    {
        public List<PontoPerfilInput> PontosPerfil { get; set; } = new List<PontoPerfilInput>();
        public string TipoPerfil { get; set; }
        public string Unidade { get; set; }
        public TipoProfundidade TipoProfundidade { get; set; }

    }
}
