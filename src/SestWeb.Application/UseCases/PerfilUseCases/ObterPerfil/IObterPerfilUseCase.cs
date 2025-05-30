using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfil
{
    public interface IObterPerfilUseCase
    {
        Task<ObterPerfilOutput> Execute(string id);
    }
}