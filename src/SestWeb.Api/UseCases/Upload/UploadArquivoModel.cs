using SestWeb.Domain.Importadores.Shallow;

namespace SestWeb.Api.UseCases.Upload
{
    public class UploadArquivoModel
    {
        public UploadArquivoModel(string mensagem, string caminho, RetornoDTO retorno)
        {
            Mensagem = mensagem;
            Caminho = caminho;
            Retorno = retorno;
        }

        /// <summary>
        /// Mensagem de retorno.
        /// </summary>
        public string Mensagem { get; }
        /// <summary>
        /// Caminho do arquivo.
        /// </summary>
        public string Caminho { get; }
        /// <summary>
        /// Dados lidos do arquivo.
        /// </summary>
        public RetornoDTO Retorno { get; set; }
    }
}
