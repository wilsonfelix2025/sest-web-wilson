using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.Repositories
{
    public interface IWellReadOnlyRepository
    {
        Task<bool> HasWell(string id);
        Task<List<Well>> GetWells();
        Task<Well> GetWell(string id);
        Task<bool> HasWellWithTheSameName(string name);
        Task<Well> GetWellByName(string name, string oilFieldId);
    }
}
