using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroLinhaBaseFolhelho
{
    public interface ICriarFiltroLinhaBaseFolhelhoUseCase
    {
        Task<CriarFiltroOutput> Execute(CriarFiltroLinhaBaseFolhelhoInput input);

    }
}
