using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.SapataUseCases.ObterSapatas
{
    public interface IObterSapatasUseCase
    {
        Task<ObterSapatasOutput> Execute(string id);
    }
}