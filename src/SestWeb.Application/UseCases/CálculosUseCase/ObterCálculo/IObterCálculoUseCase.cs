using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.ObterCálculo
{
    public interface IObterCálculoUseCase
    {
        Task<ObterCálculoOutput> Execute(string idCálculo, string idPoço);
    }
}
