using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.TrendUseCases.ObterTrend
{
    public interface IObterTrendUseCase
    {
        Task<ObterTrendOutput> Execute(string idPerfil);
    }
}
