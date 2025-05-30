using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.TrendUseCases.RemoverTrend
{
    public interface IRemoverTrendUseCase
    {
        Task<RemoverTrendOutput> Execute(string idPerfil);
    }
}
