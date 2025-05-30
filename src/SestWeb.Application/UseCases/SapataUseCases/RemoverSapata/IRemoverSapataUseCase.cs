using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.SapataUseCases.RemoverSapata
{
    public interface IRemoverSapataUseCase
    {
        Task<RemoverSapataOutput> Execute(string id, double profundidadeMedida);
    }
}