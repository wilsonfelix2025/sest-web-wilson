using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologias
{
    public interface IObterLitologiasUseCase
    {
        Task<ObterLitologiasOutput> Execute(string idPoço);
    }
}