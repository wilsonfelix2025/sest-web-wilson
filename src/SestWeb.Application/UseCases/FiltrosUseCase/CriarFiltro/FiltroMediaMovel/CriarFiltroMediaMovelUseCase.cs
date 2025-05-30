
using SestWeb.Application.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.Factory;
using SestWeb.Domain.Entities.Perfis.Factory;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroMediaMovel
{
    internal class CriarFiltroMediaMovelUseCase : ICriarFiltroMediaMovelUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IFiltroMédiaMóvelFactory _filtroMédiaMóvelFactory;

        public CriarFiltroMediaMovelUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IFiltroMédiaMóvelFactory filtroMédiaMóvelFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _filtroMédiaMóvelFactory = filtroMédiaMóvelFactory;
        }

        public async Task<CriarFiltroOutput> Execute(CriarFiltroMediaMovelInput input)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfil);
                if (perfil == null)
                    return CriarFiltroOutput.FiltroNãoCriado("Filtro não pode ser criado. Perfil não encontrado");

                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfil.IdPoço);
                var perfilSaída = PerfisFactory.Create(perfil.Mnemonico, input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());

                var result = _filtroMédiaMóvelFactory.CreateFiltroMédiaMóvel(input.Nome, "FiltroMédiaMóvel", perfil, perfilSaída, poço.Trajetória, null,
                    input.LimiteInferior, input.LimiteSuperior, input.TipoCorte, input.NúmeroPontos, out var filtro);


                if (result.IsValid)
                {
                    var outputPerfil = filtro.PerfisSaída.Perfis.First();
                    //outputPerfil.PontosDePerfil.pontos = outputPerfil.PontosDePerfil.pontos.OrderBy(p => p.Pm).ToList();
                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)filtro, "Filtros");
                    return CriarFiltroOutput.FiltroCriado(outputPerfil, (Cálculo)filtro);
                }

                return CriarFiltroOutput.FiltroNãoCriado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return CriarFiltroOutput.FiltroNãoCriado(e.Message);
            }
        }
    }
}
