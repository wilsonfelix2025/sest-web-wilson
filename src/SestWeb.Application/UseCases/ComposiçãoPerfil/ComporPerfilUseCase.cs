using SestWeb.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.ComposiçãoPerfil;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.UseCases.ComposiçãoPerfil
{
    public class ComporPerfilUseCase : IComporPerfilUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public ComporPerfilUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<ComporPerfilOutput> Execute(ComporPerfilInput input, string idPoço)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(idPoço);
                if (poço == null)
                    return ComporPerfilOutput.ComporPerfilComFalhaDeValidação("Poço não encontrado.");

                var perfil = PerfisFactory.Create(input.TipoPerfil, input.NomePerfil, poço.Trajetória, poço.ObterLitologiaPadrão());
                var montadorDePerfil = new CompositorDePerfil(poço, perfil);

                foreach (var dado in input.Lista)
                {
                    var perfilTrecho = await _perfilReadOnlyRepository.ObterPerfil(dado.IdPerfil);
                    montadorDePerfil.AdicionarTrecho(perfilTrecho, dado.PmTopo, dado.PmBase);
                }
                PerfilBase novoPerfil = montadorDePerfil.ComporPerfil();

                await _perfilWriteOnlyRepository.CriarPerfil(poço.Id, novoPerfil, poço);

                return ComporPerfilOutput.ComporPerfilComSucesso(novoPerfil);

            }
            catch (Exception e)
            {
                return ComporPerfilOutput.ComporPerfilComFalha(e.Message);
            }

        }
    }
}
