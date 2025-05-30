using SestWeb.Domain.Entities.Trajetoria;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.TrajetóriaUseCases.EditarTrajetória
{
    public interface IEditarTrajetóriaUseCase
    {
        Task<EditarTrajetóriaOutput> Execute(string idPoço, PontoTrajetóriaInput[] pontosDeTrajetória);
    }
}