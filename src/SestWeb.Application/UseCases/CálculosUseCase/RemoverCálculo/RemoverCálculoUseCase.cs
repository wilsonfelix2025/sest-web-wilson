using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisDeUmPoço;

namespace SestWeb.Application.UseCases.CálculosUseCase.RemoverCálculo
{
    internal class RemoverCálculoUseCase : IRemoverCálculoUseCase
    {
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;


        public RemoverCálculoUseCase(IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<RemoverCálculoOutput> Execute(string idPoço, string idCálculo)
        {
            try
            {
                var result = await _poçoWriteOnlyRepository.RemoverCálculo(idPoço, idCálculo);

                if (result)
                    return RemoverCálculoOutput.CálculoRemovido();

                return RemoverCálculoOutput.CálculoNãoEncontrado();
            }
            catch (Exception e)
            {
                return RemoverCálculoOutput.CálculoNãoRemovido(e.Message);
            }
        }
    }
}
