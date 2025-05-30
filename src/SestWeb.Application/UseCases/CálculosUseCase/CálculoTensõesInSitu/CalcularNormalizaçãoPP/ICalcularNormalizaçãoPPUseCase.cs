using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoPP
{
    public interface ICalcularNormalizaçãoPPUseCase
    {
        Task<CalcularNormalizaçãoPPOutput> Execute(CalcularNormalizaçãoPPInput input);

    }
}
