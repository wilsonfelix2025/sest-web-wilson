

using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.EditarCálculo
{
    public interface IEditarCálculoTensõesInSituUseCase
    {
        Task<EditarCálculoTensõesInSituOutput> Execute(EditarCálculoTensõesInSituInput input);
    }
}
