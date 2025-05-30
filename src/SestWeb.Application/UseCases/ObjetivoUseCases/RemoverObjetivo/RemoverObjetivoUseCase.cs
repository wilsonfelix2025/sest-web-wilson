using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.RemoverObjetivo
{
    internal class RemoverObjetivoUseCase : IRemoverObjetivoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public RemoverObjetivoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<RemoverObjetivoOutput> Execute(string id, double profundidadeMedida)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(id);

                if (!existePoço)
                    return RemoverObjetivoOutput.PoçoNãoEncontrado(id);

                var result = await _poçoWriteOnlyRepository.RemoverObjetivo(id, profundidadeMedida);

                if (result)
                    return RemoverObjetivoOutput.ObjetivoRemovido();

                return RemoverObjetivoOutput.ObjetivoNãoRemovido();
            }
            catch (Exception e)
            {
                return RemoverObjetivoOutput.ObjetivoNãoRemovido(e.Message);
            }
        }
    }
}
