namespace SestWeb.Api.UseCases.Importação.ImportarArquivo
{
    public class ImportarArquivoModel
    {
        public ImportarArquivoModel(string mensagem)
        {
            Mensagem = mensagem;
        }

        public string Mensagem { get; }

    }
}
