using SestWeb.Application.Helpers;
using System.IO;

namespace SestWeb.Application.UseCases.BaixarRelatórioUseCase
{
    public class BaixarRelatórioOutput : UseCaseOutput<BaixarRelatórioStatus>
    {
        public MemoryStream Arquivo { get; set; }
        public string NomeArquivo { get; set; }

        public BaixarRelatórioOutput()
        {
            
        }

        public static BaixarRelatórioOutput Sucesso(string mensagem, MemoryStream bytesArquivo, string nomeArquivo)
        {
            return new BaixarRelatórioOutput
            {
                Status = BaixarRelatórioStatus.Sucesso,
                Mensagem = mensagem,
                NomeArquivo = nomeArquivo,
                Arquivo = bytesArquivo
            };
        }

        public static BaixarRelatórioOutput Falha(string msg)
        {
            return new BaixarRelatórioOutput
            {
                Status = BaixarRelatórioStatus.Falha,
                Mensagem = $"{msg}"
            };
        }
    }
}
