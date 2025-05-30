using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.PoçoUseCases.RemoverPoço
{
    internal class RemoverPoçoUseCase : IRemoverPoçoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public RemoverPoçoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<RemoverPoçoOutput> Execute(string id)
        {
            try
            {
                // Primeiro verifica se o poço existe
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(id);

                if (!existePoço)
                    return RemoverPoçoOutput.PoçoNãoEncontrado(id);

                // tenta apagar poço
                var result = await _poçoWriteOnlyRepository.RemoverPoço(id);

                if (result)
                    return RemoverPoçoOutput.PoçoRemovido();

                return RemoverPoçoOutput.PoçoNãoRemovido();
            }
            catch (Exception e)
            {
                return RemoverPoçoOutput.PoçoNãoRemovido(e.Message);
            }
        }
    }
}