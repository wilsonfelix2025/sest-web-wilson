using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.EditarCálculo
{
    public interface IEditarCálculoPerfisUseCase
    {
        Task<EditarCálculoPerfisOutput> Execute(EditarCalculoPerfisInput input);
    }
}
