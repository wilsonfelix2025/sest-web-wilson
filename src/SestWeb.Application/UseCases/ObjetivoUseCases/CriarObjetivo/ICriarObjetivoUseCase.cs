using System.Threading.Tasks;
using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.CriarObjetivo
{
    public interface ICriarObjetivoUseCase
    {
        Task<CriarObjetivoOutput> Execute(string idPoço, double profundidadeMedida, TipoObjetivo tipoObjetivo);
    }
}