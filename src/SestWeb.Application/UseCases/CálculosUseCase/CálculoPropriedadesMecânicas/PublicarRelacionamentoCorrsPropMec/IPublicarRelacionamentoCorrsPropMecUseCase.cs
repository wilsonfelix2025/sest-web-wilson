using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.PublicarRelacionamentoCorrsPropMec
{
    public interface IPublicarRelacionamentoCorrsPropMecUseCase
    {
        Task<PublicarRelacionamentoCorrsPropMecOutput> Execute(string idPoço, string nome);
    }
}
