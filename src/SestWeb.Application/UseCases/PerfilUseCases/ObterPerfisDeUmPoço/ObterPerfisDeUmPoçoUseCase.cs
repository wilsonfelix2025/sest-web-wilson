using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisDeUmPoço
{
    internal class ObterPerfisDeUmPoçoUseCase : IObterPerfisDeUmPoçoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public ObterPerfisDeUmPoçoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<ObterPerfisDeUmPoçoOutput> Execute(string idPoço)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);

                if(!existePoço)
                    return ObterPerfisDeUmPoçoOutput.PerfisNãoObtidos($"Não foi possível encontrar poço com id {idPoço}.");

                var perfis = await _perfilReadOnlyRepository.ObterPerfisDeUmPoço(idPoço);

                return ObterPerfisDeUmPoçoOutput.PerfisObtidos(perfis);
            }
            catch (Exception e)
            {
                return ObterPerfisDeUmPoçoOutput.PerfisNãoObtidos(e.Message);
            }
        }
    }
}
