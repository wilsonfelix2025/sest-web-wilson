using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.AdicionarPoçoApoio
{
    public interface IAdicionarPoçoApoioUseCase
    {
        Task<AdicionarPoçoApoioOutput> Execute(string idPoço, string idPoçoApoio);
    }
}