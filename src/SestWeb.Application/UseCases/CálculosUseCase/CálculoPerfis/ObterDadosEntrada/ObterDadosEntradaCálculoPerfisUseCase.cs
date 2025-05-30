using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.ObterDadosEntrada
{
    internal class ObterDadosEntradaCálculoPerfisUseCase : IObterDadosEntradaCálculoPerfisUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterDadosEntradaCálculoPerfisUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterDadosEntradaCálculoPerfisOutput> Execute(string idCálculo, string idPoço)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                var calc = poço.Cálculos.First(c => c.Id.ToString() == idCálculo);

                return ObterDadosEntradaCálculoPerfisOutput.DadosObtidos(calc);
            }
            catch (Exception e)
            {
                return ObterDadosEntradaCálculoPerfisOutput.DadosNãoObtidos(e.Message);
            }
        }
    }
}
