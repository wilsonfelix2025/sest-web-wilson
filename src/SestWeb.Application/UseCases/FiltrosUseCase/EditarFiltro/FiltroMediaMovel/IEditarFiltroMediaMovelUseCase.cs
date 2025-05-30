using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroMediaMovel
{
    public interface IEditarFiltroMediaMovelUseCase
    {
        Task<EditarFiltroOutput> Execute(EditarFiltroMediaMovelInput input);
    }
}
