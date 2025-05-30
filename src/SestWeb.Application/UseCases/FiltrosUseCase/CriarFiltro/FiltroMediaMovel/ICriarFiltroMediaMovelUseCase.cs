using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroMediaMovel
{
    public interface ICriarFiltroMediaMovelUseCase
    {
        Task<CriarFiltroOutput> Execute(CriarFiltroMediaMovelInput input);
    }
}
