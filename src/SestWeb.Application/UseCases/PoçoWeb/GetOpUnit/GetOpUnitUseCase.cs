using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;

namespace SestWeb.Application.UseCases.PoçoWeb.GetOpUnit
{
    internal class GetOpUnitUseCase : IGetOpUnitUseCase
    {
        private readonly IOpUnitReadOnlyRepository _oilFieldReadOnlyRepository;

        public GetOpUnitUseCase(IOpUnitReadOnlyRepository oilFieldReadOnlyRepository)
        {
            _oilFieldReadOnlyRepository = oilFieldReadOnlyRepository;
        }
        public async Task<GetOpUnitOutput> Execute(string id)
        {
            try
            {
                OpUnit oilField = await _oilFieldReadOnlyRepository.GetOpUnit(id);

                if (oilField == null)
                {
                    return GetOpUnitOutput.OpUnitNotFound(id);
                }

                return GetOpUnitOutput.OpUnitFoundSuccessfully(oilField);
            }
            catch (Exception ex)
            {
                return GetOpUnitOutput.OpUnitNotObtained(ex.Message);
            }
        }
    }
}
