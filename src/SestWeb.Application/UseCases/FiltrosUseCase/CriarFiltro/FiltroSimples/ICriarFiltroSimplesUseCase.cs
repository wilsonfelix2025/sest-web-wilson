using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroSimples
{
    public interface ICriarFiltroSimplesUseCase
    {
        Task<CriarFiltroOutput> Execute(CriarFiltroSimplesInput input);
    }
}
