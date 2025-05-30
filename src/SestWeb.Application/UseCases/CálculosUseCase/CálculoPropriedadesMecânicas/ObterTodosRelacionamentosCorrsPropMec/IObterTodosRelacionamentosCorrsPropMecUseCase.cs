using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodosRelacionamentosCorrsPropMec
{
    public interface IObterTodosRelacionamentosCorrsPropMecUseCase
    {
        Task<ObterTodosRelacionamentosCorrsPropMecOutput> Execute(string idPoço);
    }
}
