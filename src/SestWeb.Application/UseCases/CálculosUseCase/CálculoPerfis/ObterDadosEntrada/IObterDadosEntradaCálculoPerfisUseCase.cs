using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.ObterDadosEntrada
{
    public interface IObterDadosEntradaCálculoPerfisUseCase
    {
        Task<ObterDadosEntradaCálculoPerfisOutput> Execute(string idCálculo, string idPoço);
    }
}
