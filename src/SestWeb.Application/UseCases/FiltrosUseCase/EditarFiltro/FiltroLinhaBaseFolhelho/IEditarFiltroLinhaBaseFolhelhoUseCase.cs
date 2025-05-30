using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroLinhaBaseFolhelho
{
    public interface IEditarFiltroLinhaBaseFolhelhoUseCase
    {
        Task<EditarFiltroOutput> Execute(EditarFiltroLinhaBaseFolhelhoInput input);

    }
}
