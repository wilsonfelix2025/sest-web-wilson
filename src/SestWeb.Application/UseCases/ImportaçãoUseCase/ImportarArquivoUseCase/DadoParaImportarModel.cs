using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase
{
    public abstract class DadoParaImportarModel
    {
        public TipoDeAção Ação { get; set; }
        public string Nome { get; set; }
        public string NovoNome { get; set; }
        public double? ValorTopo { get; set; }
        public double? ValorBase { get; set; }
    }
}
