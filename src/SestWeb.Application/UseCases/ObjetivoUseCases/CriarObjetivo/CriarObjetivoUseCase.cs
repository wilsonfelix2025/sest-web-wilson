using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.CriarObjetivo
{
    internal class CriarObjetivoUseCase : ICriarObjetivoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public CriarObjetivoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<CriarObjetivoOutput> Execute(string idPoço, double profundidadeMedida, TipoObjetivo tipoObjetivo)
        {
            try
            {
                if (await _poçoReadOnlyRepository.ExisteObjetivoNaProfundidade(idPoço, profundidadeMedida))
                {
                    return CriarObjetivoOutput.ObjetivoJáExiste(profundidadeMedida);
                }

                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);
                var objetivoFactory = poço.ObterObjetivoFactory();
                var objetivo = objetivoFactory.CriarObjetivo(profundidadeMedida, tipoObjetivo);

                var result = await _poçoWriteOnlyRepository.CriarObjetivo(idPoço, objetivo);

                if (result)
                {
                    return CriarObjetivoOutput.ObjetivoCriado();
                }

                return CriarObjetivoOutput.ObjetivoNãoCriado();
            }
            catch (Exception e)
            {
                return CriarObjetivoOutput.ObjetivoNãoCriado(e.Message);
            }
        }
    }
}
