using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.LitologiaUseCases.RemoverLitologia
{
    internal class RemoverLitologiaUseCase : IRemoverLitologiaUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public RemoverLitologiaUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<RemoverLitologiaOutput> Execute(string idPoço, string idLitologia)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return RemoverLitologiaOutput.PoçoNãoEncontrado(idPoço);
                }

                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);
                var litoParaRemover = poço.Litologias.Where(lito => lito.Id.ToString() == idLitologia).First();

                litoParaRemover.Clear();

                var result = await _poçoWriteOnlyRepository.AtualizarPoço(poço);

                if (result)
                    return RemoverLitologiaOutput.LitologiaRemovida();

                return RemoverLitologiaOutput.LitologiaNãoRemovida();

            }
            catch (Exception e)
            {
                return RemoverLitologiaOutput.LitologiaNãoRemovida(e.Message);
            }
        }
    }
}
