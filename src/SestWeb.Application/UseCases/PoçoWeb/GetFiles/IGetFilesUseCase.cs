using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.GetFiles
{
    public interface IGetFilesUseCase
    {
        Task<GetFilesOutput> Execute(string token, string tipoArquivo);
    }
}
