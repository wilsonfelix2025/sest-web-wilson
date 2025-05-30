using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.DuplicateFile
{
    public interface IDuplicateFileUseCase
    {
        Task<DuplicateFileOutput> Execute(string authorization, string id);
    }
}
