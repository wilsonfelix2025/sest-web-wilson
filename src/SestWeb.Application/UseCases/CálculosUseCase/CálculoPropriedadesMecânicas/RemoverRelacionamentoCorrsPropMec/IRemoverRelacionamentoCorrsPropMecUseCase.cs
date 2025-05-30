using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMec
{
    public interface IRemoverRelacionamentoCorrsPropMecUseCase
    {
        Task<RemoverRelacionamentoCorrsPropMecOutput> Execute(string nome);
    }
}
