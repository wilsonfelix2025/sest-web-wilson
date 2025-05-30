using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.RemoverItemEstratigrafia
{
    public interface IRemoverItemEstratigrafiaUseCase
    {
        Task<RemoverItemEstratigrafiaOutput> Execute(string idPoço, string tipo, double pm);
    }
}