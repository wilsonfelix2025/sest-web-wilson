using SestWeb.Application.DebugRepositories;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Infra.MongoDataAccess.DebugRepositories
{
    internal class DebugRepository : IDebugRepository
    {
        private readonly Context _context;

        public DebugRepository(Context context)
        {
            _context = context;
        }

        public async Task ApagarBancoDadosAsync()
        {
            await _context.DeletarBancoDadosAsync();
        }

        public async Task CargaInicial(List<OpUnit> opUnits, List<OilField> oilFields, List<Well> wells)
        {
            await _context.CargaInicial(opUnits, oilFields, wells);
        }

        public async Task<string> CarregarBancoComPoçoWeb(List<OpUnit> opUnits, List<OilField> oilFields, List<Well> wells)
        {
            return await _context.AlimentarBancoPoçoWeb(opUnits, oilFields, wells);
        }
    }
}