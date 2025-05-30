using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroCorte
{
    public interface IEditarFiltroCorteUseCase
    {
        Task<EditarFiltroOutput> Execute(EditarFiltroCorteInput input);
    }
}
