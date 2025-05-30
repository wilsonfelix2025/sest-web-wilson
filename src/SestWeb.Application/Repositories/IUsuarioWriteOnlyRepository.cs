using SestWeb.Domain.Usuario;
using System.Threading.Tasks;

namespace SestWeb.Application.Repositories
{
    public interface IUsuarioWriteOnlyRepository
    {
        Task<Usuario> AddUser(string email, string primeiroNome, string segundoNome, string senha);
        Task<bool> ConfirmarEmail(string idUsuario, string codigo);
        Task<string> EnviarEmailDeRecuperaçãoSenha(string email);
        Task<bool> ResetarSenha(string email, string novaSenha, string codigo);
        Task<bool> TrocarSenha(string idUsuario, string senhaAntiga, string novaSenha);
        Task<bool> TornarAdmin(string email);
        Task<bool> RemoveUser(string email);
        Task<bool> ReenviarEmail(string email);
    }
}
