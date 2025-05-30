using SestWeb.Application.Helpers;
using SestWeb.Domain.Importadores.Shallow;

namespace SestWeb.Application.UseCases.UploadUseCase
{
    public class UploadArquivoOutput : UseCaseOutput<UploadArquivoStatus>
    {
        private UploadArquivoOutput()
        {
        }

        public string Caminho { get; private set; }
        public RetornoDTO Retorno { get; set; }

        public static UploadArquivoOutput UploadRealizado(string caminho, RetornoDTO retorno)
        {
            return new UploadArquivoOutput
            {
                Status = UploadArquivoStatus.UploadRealizado,
                Mensagem = "Upload realizado com sucesso.",
                Caminho = caminho,
                Retorno = retorno
            };
        }
        
        public static UploadArquivoOutput UploadNãoRealizado(string mensagem)
        {
            return new UploadArquivoOutput
            {
                Status = UploadArquivoStatus.UploadNãoRealizado,
                Mensagem = $"Não foi possível realizar upload. {mensagem}"
            };
        }
    }
}
