using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    public class ImportarDadosPerfilRequest
    {
        public string PoçoId { get; set; }
        public List<PerfilRequest> Perfis { get; set; }
    }
}
