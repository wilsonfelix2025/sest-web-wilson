using SestWeb.Domain.Enums;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    public class ImportarDadosGenericRequest
    {
        public TipoDeAção Ação { get; set; }
        public string Nome { get; set; }
        public string NovoNome { get; set; }
        public double? ValorTopo { get; set; }
        public double? ValorBase { get; set; }
    }
}
