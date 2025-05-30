using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ExportarArquivoUseCase
{
    public interface IExportarArquivoUseCase
    {
        Task<ExportarArquivoOutput> Execute(ExportarArquivoInput input);
    }
}
