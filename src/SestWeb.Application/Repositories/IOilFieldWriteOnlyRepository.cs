using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OilField;

namespace SestWeb.Application.Repositories
{
    public interface IOilFieldWriteOnlyRepository
    {
        Task CreateOilField(OilField oilField);
        Task<bool> DeleteOilField(string id);
        Task<bool> UpdateOilField(OilField oilField);
    }
}
