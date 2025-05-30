using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais
{
    public interface IAtualizarDadosGeraisUseCase
    {
        Task<AtualizarDadosGeraisOutput> Execute(string id, AtualizarDadosGeraisInput input);
    }
}