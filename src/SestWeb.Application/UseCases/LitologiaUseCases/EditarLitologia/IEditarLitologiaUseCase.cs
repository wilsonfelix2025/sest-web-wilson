using SestWeb.Domain.Entities.LitologiaDoPoco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.LitologiaUseCases.EditarLitologia
{
    public interface IEditarLitologiaUseCase
    {
        Task<EditarLitologiaOutput> Execute(string idPoço, string tipoLitologia, Dictionary<string, string>[] pontos);
    }
}