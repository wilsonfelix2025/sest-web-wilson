using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.MoveFile
{
    public interface IMoveFileUseCase
    {
        Task<MoveFileOutput> Execute(string fileId, string wellId);
    }
}
