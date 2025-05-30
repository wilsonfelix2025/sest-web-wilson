using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PmPvUseCases
{
    public interface IPmPvUseCase
    {
        Task<PmPvOutput> Execute(string profType);
    }
}
