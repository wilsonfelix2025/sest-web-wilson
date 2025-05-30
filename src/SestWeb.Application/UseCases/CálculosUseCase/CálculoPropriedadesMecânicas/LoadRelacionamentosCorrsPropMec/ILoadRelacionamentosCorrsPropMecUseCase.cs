using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.LoadRelacionamentosCorrsPropMec
{
    public interface ILoadRelacionamentosCorrsPropMecUseCase
    {
        Task<LoadRelacionamentosCorrsPropMecOutput> Execute(string idPoço);
    }
}
