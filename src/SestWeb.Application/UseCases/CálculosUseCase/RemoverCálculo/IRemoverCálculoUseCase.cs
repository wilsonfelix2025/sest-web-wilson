using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.RemoverCálculo
{
    public interface IRemoverCálculoUseCase
    {
        Task<RemoverCálculoOutput> Execute(string idPoço, string idCálculo);
    }
}
