using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.UploadUseCase
{
    public interface IUploadArquivoUseCase
    {
        Task<UploadArquivoOutput> Execute(string extensão, byte[] stream);
    }
}