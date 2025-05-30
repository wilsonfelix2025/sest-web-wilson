using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.GetOpUnit
{
    public interface IGetOpUnitUseCase
    {
        /// <summary>
        /// Caso de uso para obter unidade operacional.
        /// </summary>
        /// <param name="id">Id da unidade operacional</param>
        /// <returns></returns>
        Task<GetOpUnitOutput> Execute(string id);
    }
}
