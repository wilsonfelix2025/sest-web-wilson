using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.DeleteFile
{
    public interface IDeleteFileUseCase
    {
        Task<DeleteFileOutput> Execute(string authorization, string id);
    }
}