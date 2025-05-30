using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.TrajetóriaUseCases.ObterTrajetória
{
    public interface IObterTrajetóriaUseCase
    {
        Task<ObterTrajetóriaOutput> Execute(string id);
    }
}