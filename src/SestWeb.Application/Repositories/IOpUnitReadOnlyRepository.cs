using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;

namespace SestWeb.Application.Repositories
{
    public interface IOpUnitReadOnlyRepository
    {
        Task<bool> HasOpUnit(string id);
        Task<List<OpUnit>> GetOpUnits();
        Task<OpUnit> GetOpUnit(string id);
        Task<bool> HasOpUnitWithTheSameName(string name);
        Task<OpUnit> GetOpUnitByName(string name);
        string GetLastId();

    }
}
