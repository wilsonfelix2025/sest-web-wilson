using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPorTipoPropMec
{
    public interface IObterCorrelaçõesPorTipoPropMecUseCase
    {
        Task<ObterCorrelaçõesPorTipoPropMecOutput> Execute(string idPoço, string grupoLitológico, string mnemônico);
    }
}
