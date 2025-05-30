using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.TrajetóriaUseCases.ObterTrajetória
{
    internal class ObterTrajetóriaUseCase : IObterTrajetóriaUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterTrajetóriaUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterTrajetóriaOutput> Execute(string id)
        {
            try
            {
                var trajetória = await _poçoReadOnlyRepository.ObterTrajetória(id);

                if (trajetória == null)
                    return ObterTrajetóriaOutput.PoçoNãoEncontrado(id);

                return ObterTrajetóriaOutput.TrajetóriaObtida(trajetória);
            }
            catch (Exception e)
            {
                return ObterTrajetóriaOutput.TrajetóriaNãoObtida(e.Message);
            }
        }
    }
}
