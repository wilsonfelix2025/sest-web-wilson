using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.DebugRepositories;
using SestWeb.Application.Services;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.DebugUseCases.AlimentarBancoComPocoWeb
{
    public class AlimentarBancoComPoçoWebUseCase : IAlimentarBancoComPoçoWebUseCase
    {
        private readonly IDebugRepository _debugRepository;
        private readonly IPocoWebService _poçoWebService;

        public AlimentarBancoComPoçoWebUseCase(IDebugRepository debugRepository, IPocoWebService pocoWebService)
        {
            _debugRepository = debugRepository;
            _poçoWebService = pocoWebService;
        }

        public async Task<string> Execute(string authorization)
        {
            await _debugRepository.ApagarBancoDadosAsync();
            List<OpUnit> opUnits = await _poçoWebService.GetOpUnitsFromWeb(authorization);
            List<OilField> oilFields = await _poçoWebService.GetOilFieldsFromWeb(authorization, opUnits);
            List<Well> wells = await _poçoWebService.GetWellsFromWeb(authorization, oilFields);
            return await _debugRepository.CarregarBancoComPoçoWeb(opUnits, oilFields, wells);
        }
    }
}