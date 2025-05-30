using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfil
{
    internal class ObterPerfilUseCase : IObterPerfilUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public ObterPerfilUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<ObterPerfilOutput> Execute(string id)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(id);

                if (perfil == null)
                {
                    return ObterPerfilOutput.PerfilNãoEncontrado(id);
                }

                return ObterPerfilOutput.PerfilObtido(perfil);
            }
            catch (Exception e)
            {
                return ObterPerfilOutput.PerfilNãoObtido(e.Message);
            }
        }
    }
}
