using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Factory;

namespace SestWeb.Application.UseCases.PerfilUseCases.CriarPerfil
{
    internal class CriarPerfilUseCase : ICriarPerfilUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;

        public CriarPerfilUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
        }

        public async Task<CriarPerfilOutput> Execute(string idPoço, string nome, string mnemônico)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return CriarPerfilOutput.PoçoNãoEncontrado(idPoço);
                }

                var trajetória = await _poçoReadOnlyRepository.ObterTrajetória(idPoço);
                var litologias = await _poçoReadOnlyRepository.ObterLitologias(idPoço);
                var litologia = litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada);

                var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);

                await _perfilWriteOnlyRepository.CriarPerfil(idPoço, perfil);

                return CriarPerfilOutput.PerfilCriado(perfil);
            }
            catch (Exception e)
            {
                return CriarPerfilOutput.PerfilNãoCriado(e.Message);
            }
        }
    }
}
