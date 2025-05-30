using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.TrendUseCases.CriarTrend
{
    internal class CriarTrendUseCase : ICriarTrendUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public CriarTrendUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<CriarTrendOutput> Execute(string idPerfil)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                if (perfil == null)
                    return CriarTrendOutput.TrendNãoCriado("Perfil não encontrado");

                var poço = await _poçoReadOnlyRepository.ObterPoço(perfil.IdPoço);

                var factory = new TrendFactory();
                var result = factory.CriarTrend(perfil, poço);

                if (result.result.IsValid == false)
                    return CriarTrendOutput.TrendNãoCriado(string.Join("\n", result.result.Errors));

                var trend = (Domain.Entities.Trend.Trend)result.Entity;

                perfil.Trend = trend;
                await _perfilWriteOnlyRepository.AtualizarTrendDoPerfil(perfil);

                return CriarTrendOutput.TrendCriado(trend);
            }
            catch (Exception e)
            {
                return CriarTrendOutput.TrendNãoCriado(e.Message);
            }
        }
    }
}
