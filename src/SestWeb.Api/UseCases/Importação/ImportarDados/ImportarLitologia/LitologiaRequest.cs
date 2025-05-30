using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase;
using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    public class LitologiaRequest : ImportarDadosGenericRequest
    {
        public string Tipo { get; set; }
        public TipoProfundidade TipoProfundidade { get; set; }
        public List<PontoLitologiaRequest> PontosLitologia { get; set; } = new List<PontoLitologiaRequest>();
        public string CorreçãoMesaRotativa { get; set; }
    }
}
