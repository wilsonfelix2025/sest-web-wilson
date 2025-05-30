using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.TrendUseCases.ObterTrend
{
    internal class ObterTrendUseCase : IObterTrendUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public ObterTrendUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<ObterTrendOutput> Execute(string idPerfil)
        {
            try
            {
                var trend = await _perfilReadOnlyRepository.ObterTrendDoPerfil(idPerfil);
                if (trend == null)
                    return ObterTrendOutput.TrendNãoObtido("Trend não encontrado");

                return ObterTrendOutput.TrendObtido(trend);
            }
            catch (Exception e)
            {
                return ObterTrendOutput.TrendNãoObtido(e.Message);
            }
        }
    }
}
