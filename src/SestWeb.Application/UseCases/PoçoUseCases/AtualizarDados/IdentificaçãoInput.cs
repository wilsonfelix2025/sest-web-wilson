namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados
{
    public class IdentificaçãoInput
    {
        public string Nome { get; set; } = string.Empty;
        public string Sonda { get; set; } = string.Empty;
        public string Campo { get; set; } = string.Empty;
        public string Companhia { get; set; } = string.Empty;
        public string Bacia { get; set; } = string.Empty;
        public string Finalidade { get; set; } = string.Empty;
        public string Analista { get; set; } = string.Empty;
        public string NívelProteção { get; set; } = string.Empty;
        public string ClassificaçãoPoço { get; set; } = string.Empty;
        public string TipoCompletação { get; set; } = string.Empty;
        public string ComplexidadePoço { get; set; } = string.Empty;
        public string VidaÚtilPrevista { get; set; } = string.Empty;
        public bool? CriticidadePoço { get; set; } 
        public bool? IntervençãoWorkover { get; set; }
    }
}
