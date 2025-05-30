using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroLitologia
{
    public interface IEditarFiltroLitologiaUseCase
    {
        Task<EditarFiltroOutput> Execute(EditarFiltroLitologiaInput input);
    }
}
