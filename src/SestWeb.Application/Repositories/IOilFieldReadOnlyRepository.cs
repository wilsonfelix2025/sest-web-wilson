using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OilField;

namespace SestWeb.Application.Repositories
{
    public interface IOilFieldReadOnlyRepository
    {
        Task<bool> HasOilField(string id);
        Task<List<OilField>> GetOilFields();
        Task<OilField> GetOilField(string id);
        Task<bool> HasOilFieldWithTheSameName(string name);
        Task<OilField> GetOilFieldByName(string name, string optUnitId);
        string GetLastId();

    }
}
