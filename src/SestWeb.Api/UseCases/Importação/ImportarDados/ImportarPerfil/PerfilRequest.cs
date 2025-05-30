using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase;
using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    public class PerfilRequest : ImportarDadosGenericRequest
    {
        public List<PontoPerfilRequest> PontosPerfil { get; set; } = new List<PontoPerfilRequest>();
        public string Tipo { get; set; }
        public string Unidade { get; set; }
        public TipoProfundidade TipoProfundidade { get; set; }

    }
}
