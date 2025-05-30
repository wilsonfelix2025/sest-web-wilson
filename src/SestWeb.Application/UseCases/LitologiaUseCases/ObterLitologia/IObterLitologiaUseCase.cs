using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologia
{
    public interface IObterLitologiaUseCase
    {
        Task<ObterLitologiaOutput> Execute(string idPoço, string idLitologia);
    }
}