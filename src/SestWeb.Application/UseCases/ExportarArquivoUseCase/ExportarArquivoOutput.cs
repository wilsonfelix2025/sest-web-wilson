using SestWeb.Application.Helpers;
using System.IO;

namespace SestWeb.Application.UseCases.ExportarArquivoUseCase
{
    public class ExportarArquivoOutput : UseCaseOutput<ExportarArquivoStatus>
    {
        public MemoryStream Arquivo { get; set; }

        public string NomeArquivo { get; set; }

        public ExportarArquivoOutput()
        {
            
        }

        public static ExportarArquivoOutput ExportaçãoConcluída(string msg, MemoryStream bytesArquivo, string nomeArquivo)
        {
            return new ExportarArquivoOutput
            {
                Status = ExportarArquivoStatus.ExportaçãoBemSucedida,
                Mensagem = $"\n{msg}",
                NomeArquivo = nomeArquivo,
                Arquivo = bytesArquivo
            };
        }

        public static ExportarArquivoOutput RequestInválido(string msg)
        {
            return new ExportarArquivoOutput
            {
                Status = ExportarArquivoStatus.ExportaçãoNãoConcluída,
                Mensagem = $"Erro de validação: {msg}"
            };
        }

        public static ExportarArquivoOutput Erro(string msg)
        {
            return new ExportarArquivoOutput
            {
                Status = ExportarArquivoStatus.ExportaçãoFalhou,
                Mensagem = $"Falha interna: {msg}"
            };
        }
    }
}
