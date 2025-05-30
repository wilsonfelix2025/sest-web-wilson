using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroSimples
{
    public interface IEditarFiltroSimplesUseCase
    {
        Task<EditarFiltroOutput> Execute(EditarFiltroSimplesInput input);
    }
}
