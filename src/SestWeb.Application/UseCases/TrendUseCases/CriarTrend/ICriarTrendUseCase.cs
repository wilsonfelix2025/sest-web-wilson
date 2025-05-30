using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.TrendUseCases.CriarTrend
{
    public interface ICriarTrendUseCase
    {
        Task<CriarTrendOutput> Execute(string idPerfil);
    }
}
