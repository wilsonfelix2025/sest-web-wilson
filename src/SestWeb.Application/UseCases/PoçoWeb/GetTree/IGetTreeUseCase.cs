using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.GetTree
{
    public interface IGetTreeUseCase
    {
        /// <summary>
        /// Caso de uso para obter árvore.
        /// </summary>
        /// <returns></returns>
        Task<GetTreeOutput> Execute();
    }
}
