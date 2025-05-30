using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoUseCases.RenomearPoço
{
    public interface IRenomearPoçoUseCase
    {
        Task<RenomearPoçoOutput> Execute(string id, string nomePoço);
    }
}