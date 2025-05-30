using SestWeb.Domain.Enums;

namespace SestWeb.Domain.DTOs.Importação
{
    public abstract class DadoParaImportarDTO
    {
        public TipoDeAção Ação { get; set; }
        public string Nome { get; set; }
        public string NovoNome { get; set; }
        public double? ValorTopo { get; set; }
        public double? ValorBase { get; set; }
    }
}
