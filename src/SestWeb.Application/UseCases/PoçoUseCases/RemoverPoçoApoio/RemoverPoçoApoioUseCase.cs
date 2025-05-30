using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.PoçoUseCases.RemoverPoçoApoio
{
    internal class RemoverPoçoApoioUseCase : IRemoverPoçoApoioUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public RemoverPoçoApoioUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<RemoverPoçoApoioOutput> Execute(string idPoço, string idPoçoApoio)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                if (poço == null)
                    return RemoverPoçoApoioOutput.SemSucesso("Poço não encontrado");

                poço.RemoverPoçoApoio(idPoçoApoio);

                await _poçoWriteOnlyRepository.AtualizarListaPoçoApoio(poço.Id, poço.IdsPoçosApoio);

                return RemoverPoçoApoioOutput.ComSucesso();
            }
            catch (Exception e)
            {
                return RemoverPoçoApoioOutput.SemSucesso(e.Message);
            }
        }

    }
}