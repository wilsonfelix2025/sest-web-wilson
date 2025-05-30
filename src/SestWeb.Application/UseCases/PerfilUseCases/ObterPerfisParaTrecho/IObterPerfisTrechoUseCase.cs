
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho
{
    public interface IObterPerfisTrechoUseCase
    {
        Task<ObterPerfisTrechoOutput> Execute(string idPoço);

    }
}
