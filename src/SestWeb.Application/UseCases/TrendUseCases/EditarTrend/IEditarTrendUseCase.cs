using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.TrendUseCases.EditarTrend
{
    public interface IEditarTrendUseCase
    {
        Task<EditarTrendOutput> Execute(List<EditarTrechosInput> input, string idPerfil, string nomeTrend);
    }
}
