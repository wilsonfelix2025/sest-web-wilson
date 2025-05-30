
namespace SestWeb.Domain.DTOs
{
    public class IdentificaçãoDTO
    {
        public string Nome { get; set; }
        public string Sonda { get; set; } = string.Empty;
        public string Campo { get; set; } = string.Empty;
        public string Companhia { get; set; } = string.Empty;
        public string Bacia { get; set; } = string.Empty;
        public string TipoPoço { get; set; }
        public string Finalidade { get; set; } = string.Empty;
        public string Analista { get; set; } = string.Empty;
        public string NívelProteção { get; set; } = string.Empty;
        public string ClassificaçãoPoço { get; set; } = string.Empty;
        public string TipoCompletação { get; set; } = string.Empty;
        public string ComplexidadePoço { get; set; } = string.Empty;
        public string VidaÚtilPrevista { get; set; } = string.Empty;
        public string PoçoWebUrl { get; set; } = string.Empty;
        public string PoçoWebDtÚltimaAtualização { get; set; } = string.Empty;
        public string PoçoWebUrlRevisões { get; set; } = string.Empty;
        public string PoçoWebRevisãoUrl { get; set; } = string.Empty;
        public bool? CriticidadePoço { get; set; } = false;
        public bool? IntervençãoWorkover { get; set; } = false;
        public string NomePoçoWeb { get; set; } = string.Empty;
        public string NomePoço { get; set; } = string.Empty;
        public string NomePoçoLocalImportação { get; set; } = string.Empty;

    }
}
