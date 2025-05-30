using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.RemoverPoço
{
    public interface IRemoverPoçoUseCase
    {
        Task<RemoverPoçoOutput> Execute(string id);
    }
}