using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateOilField
{
    public interface ICreateOilFieldUseCase
    {
        /// <summary>
        /// Caso de uso para criação de campo.
        /// </summary>
        /// <param name="name">Nome do campo</param>
        /// <param name="opUnit">Id da unidade operacional a qual o campo pertence</param>
        /// <returns></returns>
        Task<CreateOilFieldOutput> Execute(string name, string url, string opUnitId);
    }
}
