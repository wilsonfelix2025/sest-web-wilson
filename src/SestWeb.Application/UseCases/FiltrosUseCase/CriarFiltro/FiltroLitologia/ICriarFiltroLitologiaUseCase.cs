using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroLitologia
{
    public interface ICriarFiltroLitologiaUseCase
    {
        Task<CriarFiltroOutput> Execute(CriarFiltroLitologiaInput input);
    }
}
