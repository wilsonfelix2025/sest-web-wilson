using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.SapataUseCases.RemoverSapata
{
    internal class RemoverSapataUseCase: IRemoverSapataUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public RemoverSapataUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<RemoverSapataOutput> Execute(string id, double profundidadeMedida)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(id);

                if(!existePoço)
                    return RemoverSapataOutput.PoçoNãoEncontrado(id);

                var result = await _poçoWriteOnlyRepository.RemoverSapata(id, profundidadeMedida);

                if(result)
                    return RemoverSapataOutput.SapataRemovida();

                return RemoverSapataOutput.SapataNãoRemovida();
            }
            catch (Exception e)
            {
                return RemoverSapataOutput.SapataNãoRemovida(e.Message);
            }
        }
    }
}
