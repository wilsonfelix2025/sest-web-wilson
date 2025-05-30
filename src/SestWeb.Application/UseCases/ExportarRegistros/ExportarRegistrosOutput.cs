using SestWeb.Application.Helpers;
using System.IO;

namespace SestWeb.Application.UseCases.ExportarRegistros
{
    public class ExportarRegistrosOutput : UseCaseOutput<ExportarRegistrosStatus>
    {
        public MemoryStream Arquivo { get; set; }

        public string NomeArquivo { get; set; }

        public ExportarRegistrosOutput()
        {

        }

        public static ExportarRegistrosOutput ExportaçãoConcluída(string msg, MemoryStream bytesArquivo, string nomeArquivo)
        {
            return new ExportarRegistrosOutput
            {
                Status = ExportarRegistrosStatus.ExportaçãoBemSucedida,
                Mensagem = $"\n{msg}",
                NomeArquivo = nomeArquivo,
                Arquivo = bytesArquivo
            };
        }

        public static ExportarRegistrosOutput RequestInválido(string msg)
        {
            return new ExportarRegistrosOutput
            {
                Status = ExportarRegistrosStatus.ExportaçãoNãoConcluída,
                Mensagem = $"Erro de validação: {msg}"
            };
        }

        public static ExportarRegistrosOutput Erro(string msg)
        {
            return new ExportarRegistrosOutput
            {
                Status = ExportarRegistrosStatus.ExportaçãoFalhou,
                Mensagem = $"Falha interna: {msg}"
            };
        }
    }
}
