using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.CálculosUseCase.ObterCálculo
{
    internal class ObterCálculoUseCase : IObterCálculoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterCálculoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterCálculoOutput> Execute(string idCálculo, string idPoço)
        {
            try
            {
                var calc = await _poçoReadOnlyRepository.ObterCálculo(idPoço, idCálculo);

                return ObterCálculoOutput.CálculoObtido(calc);
            }
            catch (Exception e)
            {
                return ObterCálculoOutput.CálculoNãoObtido(e.Message);
            }
        }
    }
}
