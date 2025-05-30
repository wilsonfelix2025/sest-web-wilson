using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateWell
{
    public interface ICreateWellUseCase
    {
        /// <summary>
        /// Caso de uso para criação de Poço.
        /// </summary>
        /// <param name="name">Nome do poço</param>
        /// <param name="oilFieldId">Id do campo a qual o poço pertence</param>
        /// <returns></returns>
        Task<CreateWellOutput> Execute(string name, string url, string oilFieldId);
    }
}
