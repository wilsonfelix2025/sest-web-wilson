using System.Collections.Generic;
using SestWeb.Domain.Enums;

namespace SestWeb.Api.UseCases.Importação.ImportarArquivo
{
    public class DadoParaImportarRequest
    {
        public string CaminhoDoArquivo { get; set; }
        public List<DadosSelecionadosEnum> DadosSelecionados { get; set; }
        public List<PerfilParaImportar> ListaPerfis { get; set; }
        public List<LitologiaParaImportar> ListaLitologias { get; set; }
        public string PoçoId { get; set; }
        public string CorreçãoMesaRotativa { get; set; } = "";
        public Dictionary<string, object> Extras { get; set; } = new Dictionary<string, object>();
    }
}
