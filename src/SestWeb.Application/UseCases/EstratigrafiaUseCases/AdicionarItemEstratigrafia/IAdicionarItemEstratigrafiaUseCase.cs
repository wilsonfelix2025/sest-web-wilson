using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AdicionarItemEstratigrafia
{
    public interface IAdicionarItemEstratigrafiaUseCase
    {
        Task<AdicionarItemEstratigrafiaOutput> Execute(AdicionarItemEstratigrafiaInput input);
    }
}