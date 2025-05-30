using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.ObterObjetivos
{
    public interface IObterObjetivosUseCase
    {
        Task<ObterObjetivosOutput> Execute(string id);
    }
}