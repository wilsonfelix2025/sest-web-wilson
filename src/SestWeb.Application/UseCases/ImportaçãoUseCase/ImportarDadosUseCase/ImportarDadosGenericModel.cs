using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase
{
    public class ImportarDadosGenericInput
    {
        public TipoDeAção Ação { get; set; }
        public string Nome { get; set; }
        public string NovoNome { get; set; }
        public double? ValorTopo { get; set; }
        public double? ValorBase { get; set; }
        public string CorreçãoMesaRotativa { get; set; } = "";
    }
}
