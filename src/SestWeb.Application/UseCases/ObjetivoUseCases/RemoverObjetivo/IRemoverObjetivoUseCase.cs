using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.RemoverObjetivo
{
    public interface IRemoverObjetivoUseCase
    {
        Task<RemoverObjetivoOutput> Execute(string id, double profundidadeMedida);
    }
}