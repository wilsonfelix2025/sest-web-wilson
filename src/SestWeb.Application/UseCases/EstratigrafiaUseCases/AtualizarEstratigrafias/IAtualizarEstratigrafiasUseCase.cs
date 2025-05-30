using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AtualizarEstratigrafias
{
    public interface IAtualizarEstratigrafiasUseCase
    {
        Task<AtualizarEstratigrafiasOutput> Execute(string idPoço, AtualizarEstratigrafiasInput input);
    }
}