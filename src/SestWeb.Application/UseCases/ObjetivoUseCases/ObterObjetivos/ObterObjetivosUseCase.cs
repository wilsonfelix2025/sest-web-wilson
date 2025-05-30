using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.ObterObjetivos
{
    internal class ObterObjetivosUseCase : IObterObjetivosUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterObjetivosUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterObjetivosOutput> Execute(string id)
        {
            try
            {
                var objetivos = await _poçoReadOnlyRepository.ObterObjetivos(id);

                if (objetivos == null)
                    return ObterObjetivosOutput.PoçoNãoEncontrado(id);

                return ObterObjetivosOutput.ObjetivosObtidos(objetivos);
            }
            catch (Exception e)
            {
                return ObterObjetivosOutput.ObjetivosNãoObtidos(e.Message);
            }
        }
    }
}
