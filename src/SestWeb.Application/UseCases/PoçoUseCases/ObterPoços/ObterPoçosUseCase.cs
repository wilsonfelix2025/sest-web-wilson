using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterPoços
{
    internal class ObterPoçosUseCase : IObterPoçosUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterPoçosUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterPoçosOutput> Execute()
        {
            var poços = await _poçoReadOnlyRepository.ObterPoços();

            var poçosOutput = new List<PoçoOutput>();
            foreach (var poço in poços) poçosOutput.Add(new PoçoOutput(poço.Id.ToString(), poço.Nome, poço.TipoPoço));

            return ObterPoçosOutput.PoçosObtidos(poçosOutput);
        }
    }
}