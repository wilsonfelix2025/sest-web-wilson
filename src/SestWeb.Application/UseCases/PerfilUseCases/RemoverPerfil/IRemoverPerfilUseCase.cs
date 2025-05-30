using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PerfilUseCases.RemoverPerfil
{
    public interface IRemoverPerfilUseCase
    {
        Task<RemoverPerfilOutput> Execute(string idPerfil, string idPoço);
    }
}