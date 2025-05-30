using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.EditarCálculo
{
    public interface IEditarCálculoPropMecUseCase
    {
        Task<EditarCálculoPropMecOutput> Execute(EditarCalculoPropMecInput input);
    }
}
