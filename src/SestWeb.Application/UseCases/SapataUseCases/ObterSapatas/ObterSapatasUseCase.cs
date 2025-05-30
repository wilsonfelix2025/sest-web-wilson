using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.SapataUseCases.ObterSapatas
{
    internal class ObterSapatasUseCase : IObterSapatasUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterSapatasUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterSapatasOutput> Execute(string id)
        {
            try
            {
                var sapatas = await _poçoReadOnlyRepository.ObterSapatas(id);

                if (sapatas == null)
                    return ObterSapatasOutput.PoçoNãoEncontrado(id);

                return ObterSapatasOutput.SapatasObtidas(sapatas);
            }
            catch (Exception e)
            {
                return ObterSapatasOutput.SapatasNãoObtidas(e.Message);
            }
        }
    }
}
