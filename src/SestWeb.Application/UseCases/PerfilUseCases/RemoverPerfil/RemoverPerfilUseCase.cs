using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Application.UseCases.PerfilUseCases.RemoverPerfil
{
    internal class RemoverPerfilUseCase : IRemoverPerfilUseCase
    {
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public RemoverPerfilUseCase(IPerfilWriteOnlyRepository perfilWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<RemoverPerfilOutput> Execute(string idPerfil, string idPoço)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(idPoço);
                var result = await _perfilWriteOnlyRepository.RemoverPerfil(idPerfil, poço);

                if (result)
                    return RemoverPerfilOutput.PerfilRemovido();

                return RemoverPerfilOutput.PerfilNãoEncontrado(idPerfil);
            }
            catch (Exception e)
            {
                return RemoverPerfilOutput.PerfilNãoRemovido(e.Message);
            }
        }
    }
}
