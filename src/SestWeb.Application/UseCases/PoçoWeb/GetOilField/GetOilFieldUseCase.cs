using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OilField;

namespace SestWeb.Application.UseCases.PoçoWeb.GetOilField
{
    internal class GetOilFieldUseCase : IGetOilFieldUseCase
    {
        private readonly IOilFieldReadOnlyRepository _oilFieldReadOnlyRepository;

        public GetOilFieldUseCase(IOilFieldReadOnlyRepository oilFieldReadOnlyRepository)
        {
            _oilFieldReadOnlyRepository = oilFieldReadOnlyRepository;
        }
        public async Task<GetOilFieldOutput> Execute(string id)
        {
            try
            {
                OilField oilField = await _oilFieldReadOnlyRepository.GetOilField(id);

                if (oilField == null)
                {
                    return GetOilFieldOutput.OilFieldNotFound(id);
                }

                return GetOilFieldOutput.OilFieldFoundSuccessfully(oilField);
            }
            catch (Exception ex)
            {
                return GetOilFieldOutput.OilFieldNotObtained(ex.Message);
            }
        }
    }
}
