using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados
{
    public interface IAtualizarDadosUseCase
    {
        Task<AtualizarDadosOutput> Execute(string id, AtualizarDadosInput input);

    }
}
