using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodasCorrelaçõesPossíveisPropMec
{
    public interface IObterTodasCorrelaçõesPossíveisPropMecUseCase
    {
        Task<ObterTodasCorrelaçõesPossíveisPropMecOutput> Execute(string idPoço);
    }
}
