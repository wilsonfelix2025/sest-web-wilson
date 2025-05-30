using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoLDA
{
    public interface ICalcularNormalizaçãoLDAUseCase
    {
        Task<CalcularNormalizaçãoLDAOutput> Execute(CalcularNormalizaçãoLDAInput input);

    }
}
