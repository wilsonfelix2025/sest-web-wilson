using SestWeb.Application.DebugRepositories;
using System.Threading.Tasks;

namespace SestWeb.Application.DebugUseCases.ApagarBanco
{
    internal class ApagarBancoUseCase : IApagarBancoUseCase
    {
        private readonly IDebugRepository _debugRepository;

        public ApagarBancoUseCase(IDebugRepository debugRepository)
        {
            _debugRepository = debugRepository;
        }

        public async Task Execute()
        {
            await _debugRepository.ApagarBancoDadosAsync();
        }
    }
}