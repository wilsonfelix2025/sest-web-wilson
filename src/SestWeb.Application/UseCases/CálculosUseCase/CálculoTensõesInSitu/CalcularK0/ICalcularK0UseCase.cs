using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0
{
    public interface ICalcularK0UseCase
    {
        Task<CalcularK0Output> Execute(CalcularK0Input input);
    }
}
