namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper.EmbeddedAssemblyExtraction
{
    public class ExtractionResult
    {
        public ExtractionResult(ExtractionResultStatus status, string mensagem = "")
        {
            Status = status;
            Mensagem = mensagem;
        }

        public ExtractionResultStatus Status { get; }
        public string Mensagem { get; }
    }
}
