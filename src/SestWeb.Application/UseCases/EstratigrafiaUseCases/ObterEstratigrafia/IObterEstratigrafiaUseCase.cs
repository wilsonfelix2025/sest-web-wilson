using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.ObterEstratigrafia
{
    public interface IObterEstratigrafiaUseCase
    {
        Task<ObterEstratigrafiaOutput> Execute(string idPoço);
    }
}