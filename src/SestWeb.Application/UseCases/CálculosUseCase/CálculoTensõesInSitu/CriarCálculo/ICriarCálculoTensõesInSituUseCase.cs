

using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarCálculo
{
    public interface ICriarCálculoTensõesInSituUseCase
    {
        Task<CriarCálculoTensõesInSituOutput> Execute(CriarCálculoTensõesInSituInput input);
    }
}
