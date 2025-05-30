using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateOilField
{
    internal class CreateOilFieldUseCase : ICreateOilFieldUseCase
    {
        private readonly IOilFieldWriteOnlyRepository _oilFieldWriteOnlyRepository;
        private readonly IOpUnitWriteOnlyRepository _opunitWriteOnlyRepository;
        private readonly IOpUnitReadOnlyRepository _opunitReadOnlyRepository;

        public CreateOilFieldUseCase(IOilFieldWriteOnlyRepository oilFieldWriteOnlyRepository, 
        IOpUnitWriteOnlyRepository opunitWriteOnlyRepository, IOpUnitReadOnlyRepository opunitReadOnlyRepository)
        {
            _oilFieldWriteOnlyRepository = oilFieldWriteOnlyRepository;
            _opunitWriteOnlyRepository = opunitWriteOnlyRepository;
            _opunitReadOnlyRepository = opunitReadOnlyRepository;
        }
        public async Task<CreateOilFieldOutput> Execute(string name, string url, string opUnitId)
        {
            try
            {
                OilField oilField = new OilField(url, name, opUnitId);

                OpUnit opUnit = await _opunitReadOnlyRepository.GetOpUnit(opUnitId);
                opUnit.OilFields.Add(oilField.Id);

                await _oilFieldWriteOnlyRepository.CreateOilField(oilField);

                await _opunitWriteOnlyRepository.UpdateOpUnit(opUnit);
                
                return CreateOilFieldOutput.OilFieldCreatedSuccesfully(oilField);
            }
            catch (Exception ex)
            {
                return CreateOilFieldOutput.OilFieldNotCreated(ex.Message);
            }
        }
    }
}
