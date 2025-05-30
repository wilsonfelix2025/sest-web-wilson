using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisPorTipo
{
    internal class ObterPerfisPorTipoUseCase : IObterPerfisPorTipoUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public ObterPerfisPorTipoUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<ObterPerfisPorTipoOutput> Execute(string idPoço, string mnemônico)
        {
            try
            {
                TiposPerfil.GeTipoPerfil(mnemônico);

                var perfis = await _perfilReadOnlyRepository.ObterPerfisPorTipo(idPoço, mnemônico);

                return ObterPerfisPorTipoOutput.PerfisObtidos(perfis);
            }
            catch (Exception e)
            {
                return ObterPerfisPorTipoOutput.PerfisNãoObtidos(e.Message);
            }
        }
    }
}
