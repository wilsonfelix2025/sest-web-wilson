using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;

namespace SestWeb.Application.Repositories
{
    public interface IOpUnitWriteOnlyRepository
    {
        Task CreateOpUnit(OpUnit opUnit);
        Task<bool> UpdateOpUnit(OpUnit opUnit);
        Task<bool> DeleteOpUnit(string id);
    }
}
