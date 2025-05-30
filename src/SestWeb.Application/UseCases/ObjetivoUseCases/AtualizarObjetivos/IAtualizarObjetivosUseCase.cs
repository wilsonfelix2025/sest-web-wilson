using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.AtualizarObjetivos
{
    public interface IAtualizarObjetivosUseCase
    {
        Task<AtualizarObjetivosOutput> Execute(string idPoço, AtualizarObjetivosInput input);
    }
}
