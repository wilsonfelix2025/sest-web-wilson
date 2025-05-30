using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterDadosGerais
{
    public interface IObterDadosGeraisUseCase
    {
        Task<ObterDadosGeraisOutput> Execute(string id);
    }
}
