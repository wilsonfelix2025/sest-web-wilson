using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.TrendUseCases.RemoverTrend
{
    internal class RemoverTrendUseCase : IRemoverTrendUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;

        public RemoverTrendUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<RemoverTrendOutput> Execute(string idPerfil)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                if (perfil == null)
                    return RemoverTrendOutput.TrendNãoRemovido("Perfil não encontrado");

                perfil.RemoverTrend();

                await _perfilWriteOnlyRepository.AtualizarTrendDoPerfil(perfil);

                return RemoverTrendOutput.TrendRemovido();
            }
            catch (Exception e)
            {
                return RemoverTrendOutput.TrendNãoRemovido(e.Message);
            }
        }
    }
}
