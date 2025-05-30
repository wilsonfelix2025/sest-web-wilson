using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.DebugRepositories
{
    public interface IDebugRepository
    {
        Task ApagarBancoDadosAsync();

        Task<string> CarregarBancoComPoçoWeb(List<OpUnit> opUnits, List<OilField> oilFields, List<Well> wells);

        Task CargaInicial(List<OpUnit> opUnits, List<OilField> oilFields, List<Well> wells);
    }
}