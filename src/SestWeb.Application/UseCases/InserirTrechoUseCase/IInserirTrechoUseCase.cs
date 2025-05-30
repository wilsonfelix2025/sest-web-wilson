using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.InserirTrechoUseCase
{
    public interface IInserirTrechoUseCase
    {
        Task<InserirTrechoOutput> Execute(string idPerfil, InserirTrechoInput trechoInicial);

    }
}
