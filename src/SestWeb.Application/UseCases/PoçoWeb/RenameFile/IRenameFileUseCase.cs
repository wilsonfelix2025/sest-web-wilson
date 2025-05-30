using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.RenameFile
{
    public interface IRenameFileUseCase
    {
        Task<RenameFileOutput> Execute(string id, string newName);
    }
}
