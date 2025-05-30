using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.CriarCálculo
{
    public interface ICriarCálculoPressãoPorosUseCase
    {
        Task<CriarCálculoPressãoPorosOutput> Execute(CriarCálculoPressãoPorosInput input);
    }
}
