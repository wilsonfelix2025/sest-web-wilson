using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologia
{
    internal class ObterLitologiaUseCase : IObterLitologiaUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterLitologiaUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterLitologiaOutput> Execute(string idPoço, string idLitologia)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!poço)
                {
                    return ObterLitologiaOutput.PoçoNãoEncontrado(idPoço);
                }

                var result = await _poçoReadOnlyRepository.ObterLitologia(idPoço, idLitologia);

                if (result == null)
                {
                    return ObterLitologiaOutput.LitologiaNãoObtida();
                }

                return ObterLitologiaOutput.LitologiaObtida(result);
            }
            catch (Exception e)
            {
                return ObterLitologiaOutput.LitologiaNãoObtida(e.Message);
            }
        }
    }
}
