using SestWeb.Domain.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.Repositories
{
    public interface IUsuarioReadOnlyRepository
    {
        Task<Usuario> GetUserByPassword(string email, string senha);

        Task<Usuario> GetUserById(string id);

        Task<Usuario> GetUserByEmail(string email);

        Task<IReadOnlyCollection<Usuario>> GetAll();

        Task<IReadOnlyCollection<string>> GetAdminEmails();
    }
}
