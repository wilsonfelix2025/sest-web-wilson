using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0Acompanhamento
{
    public interface ICalcularK0AcompanhamentoUseCase
    {
        Task<CalcularK0AcompanhamentoOutput> Execute(CalcularK0AcompanhamentoInput input);
    }
}
