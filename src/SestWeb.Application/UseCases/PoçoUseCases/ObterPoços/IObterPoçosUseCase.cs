using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterPoços
{
    public interface IObterPoçosUseCase
    {
        Task<ObterPoçosOutput> Execute();
    }
}