using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.SapataUseCases.CriarSapata
{
    internal class CriarSapataUseCase : ICriarSapataUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public CriarSapataUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }
        public async Task<CriarSapataOutput> Execute(string idPoço, double profundidadeMedida, string diâmetroSapata)
        {
            try
            {
                if(await _poçoReadOnlyRepository.ExisteSapataNaProfundidade(idPoço, profundidadeMedida))
                {
                    return CriarSapataOutput.SapataJáExiste(profundidadeMedida);
                }

                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);
                var sapataFactory = poço.ObterSapataFactory();

                var sapata = sapataFactory.CriarSapata(profundidadeMedida, diâmetroSapata);

                var result = await _poçoWriteOnlyRepository.CriarSapata(idPoço, sapata);

                if (result)
                {
                    return CriarSapataOutput.SapataCriada();
                }

                return CriarSapataOutput.SapataNãoCriada();
            }
            catch (Exception e)
            {
                return CriarSapataOutput.SapataNãoCriada(e.Message);
            }
        }
    }
}
