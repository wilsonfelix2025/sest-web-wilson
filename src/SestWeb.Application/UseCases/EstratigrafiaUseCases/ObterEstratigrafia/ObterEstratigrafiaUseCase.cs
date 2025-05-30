using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.ObterEstratigrafia
{
    internal class ObterEstratigrafiaUseCase : IObterEstratigrafiaUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterEstratigrafiaUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterEstratigrafiaOutput> Execute(string idPoço)
        {
            try
            {
                var estratigrafia = await _poçoReadOnlyRepository.ObterEstratigrafia(idPoço);

                if (estratigrafia == null)
                    return ObterEstratigrafiaOutput.EstratigrafiaNãoObtida($"Não foi possível obter estratigrafia do poço {idPoço}.");

                return ObterEstratigrafiaOutput.EstratigrafiaObtida(estratigrafia);
            }
            catch (Exception e)
            {
                return ObterEstratigrafiaOutput.EstratigrafiaNãoObtida(e.Message);
            }
        }
    }
}