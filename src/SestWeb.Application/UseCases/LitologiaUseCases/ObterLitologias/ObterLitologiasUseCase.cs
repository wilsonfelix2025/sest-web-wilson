using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologias
{
    internal class ObterLitologiasUseCase : IObterLitologiasUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterLitologiasUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterLitologiasOutput> Execute(string idPoço)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!poço)
                {
                    return ObterLitologiasOutput.PoçoNãoEncontrado(idPoço);
                }

                var result = await _poçoReadOnlyRepository.ObterLitologias(idPoço);

                if (result == null)
                {
                    return ObterLitologiasOutput.LitologiasNãoObtidas();
                }

                return ObterLitologiasOutput.LitologiasObtidas(result);
            }
            catch (Exception e)
            {
                return ObterLitologiasOutput.LitologiasNãoObtidas(e.Message);
            }
        }
    }
}
