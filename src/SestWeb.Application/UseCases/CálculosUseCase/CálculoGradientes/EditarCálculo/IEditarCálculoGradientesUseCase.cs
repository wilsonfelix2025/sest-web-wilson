using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.EditarCálculo
{
    public interface IEditarCálculoGradientesUseCase
    {
        Task<EditarCálculoGradientesOutput> Execute(EditarCálculoGradientesInput input);
    }
}
