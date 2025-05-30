using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.GetOilField
{
    public interface IGetOilFieldUseCase
    {
        /// <summary>
        /// Caso de uso para obter campo.
        /// </summary>
        /// <param name="id">Id do campo</param>
        /// <returns></returns>
        Task<GetOilFieldOutput> Execute(string id);
    }
}
