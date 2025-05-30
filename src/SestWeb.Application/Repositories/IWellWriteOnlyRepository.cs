using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.Repositories
{
    public interface IWellWriteOnlyRepository
    {
        Task<Well> CreateWell(WellRequest well, string authorization);
        Task<bool> UpdateWell(Well well);
        Task<bool> DeleteWell(string id);
    }
}
