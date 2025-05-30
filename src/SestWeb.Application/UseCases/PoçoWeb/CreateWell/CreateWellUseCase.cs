using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateWell
{
    internal class CreateWellUseCase : ICreateWellUseCase
    {
        private readonly IWellWriteOnlyRepository _wellWriteOnlyRepository;
        private readonly IOilFieldWriteOnlyRepository _oilFieldWriteOnlyRepository;
        private readonly IOilFieldReadOnlyRepository _oilFieldReadOnlyRepository;

        public CreateWellUseCase(IWellWriteOnlyRepository wellWriteOnlyRepository, IOilFieldWriteOnlyRepository oilFieldWriteOnlyRepository, IOilFieldReadOnlyRepository oilFieldReadOnlyRepository)
        {
            _wellWriteOnlyRepository = wellWriteOnlyRepository;
            _oilFieldWriteOnlyRepository = oilFieldWriteOnlyRepository;
            _oilFieldReadOnlyRepository = oilFieldReadOnlyRepository;
        }
        public async Task<CreateWellOutput> Execute(string authorization, string name, string oilFieldId)
        {
            try
            {
                OilField oilField = await _oilFieldReadOnlyRepository.GetOilField(oilFieldId);
                WellRequest well = new WellRequest(name, oilField.Url);

                Well newWell = await _wellWriteOnlyRepository.CreateWell(well, authorization);
                if (newWell == null)
                {
                    return CreateWellOutput.WellNotCreated("Erro ao contatar banco do PoçoWeb.");
                }

                oilField.Wells.Add(newWell.Id);
                await _oilFieldWriteOnlyRepository.UpdateOilField(oilField);

                return CreateWellOutput.WellCreatedSuccesfully(newWell);
            }
            catch (Exception ex)
            {
                return CreateWellOutput.WellNotCreated(ex.Message);
            }
        }
    }
}
