using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.GetWell
{
    public interface IGetWellUseCase
    {
        /// <summary>
        /// Caso de uso para obter poço.
        /// </summary>
        /// <param name="id">Id do poço</param>
        /// <returns></returns>
        Task<GetWellOutput> Execute(string id);
    }
}
