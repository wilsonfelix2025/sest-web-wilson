using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.Recalcular
{
    public interface IRecalcularCálculoGradientesUseCase
    {
        Task<RecalcularCálculoGradientesOutput> Execute(RecalcularCálculoGradientesInput input);
    }
}
