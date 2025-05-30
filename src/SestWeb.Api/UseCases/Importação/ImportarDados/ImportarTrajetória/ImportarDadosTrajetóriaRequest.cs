using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    public class ImportarDadosTrajetóriaRequest : ImportarDadosGenericRequest
    {
        public string PoçoId { get; set; }
        public List<PontoTrajetóriaRequest> PontosTrajetória { get; set; }

    }
}
