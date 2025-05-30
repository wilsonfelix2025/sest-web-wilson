using System.Threading.Tasks;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.Repositories
{
    public interface IPerfilWriteOnlyRepository
    {
        Task CriarPerfil(string idPoço, PerfilBase perfil, Poço poço = null);
        Task<bool> RemoverPerfil(string idPerfil, Poço poço);
        Task AtualizarPerfis(string idPoço, Poço poçoAtualizado);
        Task<bool> AtualizarPerfil(PerfilBase perfilAtualizado);
        Task AtualizarTrendDoPerfil(PerfilBase perfil);
        
    }
}