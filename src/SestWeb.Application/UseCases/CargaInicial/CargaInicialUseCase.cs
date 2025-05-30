using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.DebugRepositories;
using SestWeb.Application.Repositories;
using SestWeb.Application.Services;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.LoaderCorrelaçõesSistema;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.UseCases.CargaInicial
{
    internal class CargaInicialUseCase : ICargaInicialUseCase
    {
        private readonly IDebugRepository _debugRepository;
        private readonly IPocoWebService _poçoWebService;
        private readonly IOpUnitReadOnlyRepository _opUnitReadOnlyRepository;
        private readonly IOilFieldReadOnlyRepository _oilFieldReadOnlyRepository;
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoWriteOnlyRepository _correlaçãoWriteOnlyRepository;
        private readonly ILoaderCorrelações _loaderCorrelações;

        public CargaInicialUseCase(IDebugRepository debugRepository, IPocoWebService pocoWebService,
            IOpUnitReadOnlyRepository opUnitReadOnlyRepository, IOilFieldReadOnlyRepository oilFieldReadOnlyRepository, IWellReadOnlyRepository wellReadOnlyRepository,
            ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoWriteOnlyRepository correlaçãoWriteOnlyRepository, ILoaderCorrelações loaderCorrelações)
        {
            _debugRepository = debugRepository;
            _poçoWebService = pocoWebService;
            _opUnitReadOnlyRepository = opUnitReadOnlyRepository;
            _oilFieldReadOnlyRepository = oilFieldReadOnlyRepository;
            _wellReadOnlyRepository = wellReadOnlyRepository;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoWriteOnlyRepository = correlaçãoWriteOnlyRepository;
            _loaderCorrelações = loaderCorrelações;
        }

        public async Task Execute(string authorization)
        {
            await LoadPoçoWeb(authorization);
            await LoadCorrelações();
        }

        private async Task LoadPoçoWeb(string authorization)
        {
            List<OpUnit> pWebOpUnits = await _poçoWebService.GetOpUnitsFromWeb(authorization);
            List<OilField> pWebOilFields = await _poçoWebService.GetOilFieldsFromWeb(authorization, pWebOpUnits);
            List<Well> pWebWells = await _poçoWebService.GetWellsFromWeb(authorization, pWebOilFields);

            List<OpUnit> dbOpUnits = await _opUnitReadOnlyRepository.GetOpUnits();
            List<OilField> dbOilFields = await _oilFieldReadOnlyRepository.GetOilFields();
            List<Well> dbWells = await _wellReadOnlyRepository.GetWells();

            List<OpUnit> loadingOpUnits = pWebOpUnits.FindAll(ou => dbOpUnits.All(o => !o.Id.Equals(ou.Id)));
            List<OilField> loadingOilFields =
                pWebOilFields.FindAll(of => dbOilFields.All(o => !o.Id.Equals(of.Id)));
            List<Well> loadingWells = pWebWells.FindAll(well => dbWells.All(w => !w.Id.Equals(well.Id)));


            await _debugRepository.CargaInicial(loadingOpUnits, loadingOilFields, loadingWells);
        }

        private async Task LoadCorrelações()
        {
            var correlações = _loaderCorrelações.Load().Cast<Correlação>().ToList();

            var existeCorrelação = await _correlaçãoReadOnlyRepository.CorrelaçõesSistemaCarregadas();
            if (existeCorrelação)
            {
                foreach (Correlação correlação in correlações)
                {
                    await _correlaçãoWriteOnlyRepository.UpdateCorrelação(correlação);
                }
            }
            else
            {
                await _correlaçãoWriteOnlyRepository.InsertCorrelaçõesSistema(correlações);
            }
        }
    }
}
