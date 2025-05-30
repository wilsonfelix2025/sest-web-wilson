using SestWeb.Application.Repositories;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.UseCases.PoçoWeb.GetWell
{
    internal class GetWellUseCase : IGetWellUseCase
    {
        private readonly IWellReadOnlyRepository _wellReadOnlyRepository;

        public GetWellUseCase(IWellReadOnlyRepository wellReadOnlyRepository)
        {
            _wellReadOnlyRepository = wellReadOnlyRepository;
        }
        public async Task<GetWellOutput> Execute(string id)
        {
            try
            {
                Well well = await _wellReadOnlyRepository.GetWell(id);

                if (well == null)
                {
                    return GetWellOutput.WellNotFound(id);
                }

                return GetWellOutput.WellFoundSuccessfully(well);
            }
            catch (Exception ex)
            {
                return GetWellOutput.WellNotObtained(ex.Message);
            }
        }
    }
}
