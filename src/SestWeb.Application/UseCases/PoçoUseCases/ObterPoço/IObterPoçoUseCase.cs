using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterPoço
{
    public interface IObterPoçoUseCase
    {
        Task<ObterPoçoOutput> Execute(string id, string token);
    }
}
