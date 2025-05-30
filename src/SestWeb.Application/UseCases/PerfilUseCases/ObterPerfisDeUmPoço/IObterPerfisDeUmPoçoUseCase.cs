using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisDeUmPoço
{
    public interface IObterPerfisDeUmPoçoUseCase
    {
        Task<ObterPerfisDeUmPoçoOutput> Execute(string idPoço);
    }
}