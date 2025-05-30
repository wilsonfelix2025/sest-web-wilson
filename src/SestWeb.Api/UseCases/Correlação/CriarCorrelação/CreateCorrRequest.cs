namespace SestWeb.Api.UseCases.Correlação.CriarCorrelação
{
    public class CreateCorrRequest
    {
        public string IdPoço { get; set; }
        public string Nome { get; set; }
        public string NomeAutor { get; set; }
        public string ChaveAutor { get; set; }
        public string Descrição { get; set; }
        public string Expressão { get; set; }
    }
}
