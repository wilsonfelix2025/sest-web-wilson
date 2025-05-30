using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.CriarCálculo
{
    public interface ICriarCálculoGradientesUseCase
    {
        Task<CriarCálculoGradientesOutput> Execute(CriarCálculoGradientesInput input);
    }
}
