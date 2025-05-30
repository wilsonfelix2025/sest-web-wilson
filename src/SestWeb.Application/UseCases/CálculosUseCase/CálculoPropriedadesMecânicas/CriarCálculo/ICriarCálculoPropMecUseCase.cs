using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarCálculo
{
    public interface ICriarCálculoPropMecUseCase
    {
        Task<CriarCálculoPropMecOutput> Execute(CriarCalculoPropMecInput input);
    }
}
