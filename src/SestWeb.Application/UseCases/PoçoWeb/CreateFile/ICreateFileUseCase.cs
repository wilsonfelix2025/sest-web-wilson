using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateFile
{
    public interface ICreateFileUseCase
    {
        Task<CreateFileOutput> Execute(string authorization, string name, string wellId, string description, string fileType);
    }
}
