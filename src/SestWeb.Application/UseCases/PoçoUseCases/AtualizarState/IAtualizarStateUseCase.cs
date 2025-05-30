using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarState
{
    public interface IAtualizarStateUseCase
    {
        Task<AtualizarStateOutput> Execute(string id, AtualizarStateInput state);
    }
}