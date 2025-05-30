using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.RemoverPoçoApoio
{
    public interface IRemoverPoçoApoioUseCase
    {
        Task<RemoverPoçoApoioOutput> Execute(string idPoço, string idPoçoApoio);
    }
}