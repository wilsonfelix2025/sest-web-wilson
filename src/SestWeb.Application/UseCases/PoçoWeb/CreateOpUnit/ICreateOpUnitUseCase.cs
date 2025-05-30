using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateOpUnit
{
    public interface ICreateOpUnitUseCase
    {
        /// <summary>
        /// Caso de uso para criação de unidade operacional.
        /// </summary>
        /// <param name="name">Nome da unidade operacional</param>
        /// <returns></returns>
        Task<CreateOpUnitOutput> Execute(string name, string url);
    }
}
