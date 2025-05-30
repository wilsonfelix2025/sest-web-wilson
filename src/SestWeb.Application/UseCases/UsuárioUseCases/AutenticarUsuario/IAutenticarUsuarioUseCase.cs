using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.UsuárioUseCases.AutenticarUsuario
{
    public interface IAutenticarUsuarioUseCase
    {
        Task<AutenticarUsuarioOutput> Execute(string email, string senha);
    }
}
