using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroCorte
{
    public interface ICriarFiltroCorteUseCase
    {
        Task<CriarFiltroOutput> Execute(CriarFiltroCorteInput input);
    }
}
