using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.RegistrosEventosUseCases.ReiniciarRegistroEvento
{
    public interface IReiniciarRegistroEventoUseCase
    {
        Task<ReiniciarRegistroEventoOutput> Execute(string id);
    }
}