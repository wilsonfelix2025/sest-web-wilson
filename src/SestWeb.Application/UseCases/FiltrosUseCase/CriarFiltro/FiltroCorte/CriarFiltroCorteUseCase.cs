using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros.Corte.Factory;
using SestWeb.Domain.Entities.Perfis.Factory;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroCorte
{
    internal class CriarFiltroCorteUseCase : ICriarFiltroCorteUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IFiltroCorteFactory _filtroCorteFactory;

        public CriarFiltroCorteUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IFiltroCorteFactory filtroCorteFactory
           )
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _filtroCorteFactory = filtroCorteFactory;
        }

        public async Task<CriarFiltroOutput> Execute(CriarFiltroCorteInput input)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfil);
                if (perfil == null)
                    return CriarFiltroOutput.FiltroNãoCriado("Filtro não pode ser criado. Perfil não encontrado");

                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfil.IdPoço);

                var perfilSaída = PerfisFactory.Create(perfil.Mnemonico, input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());
                
                var result = _filtroCorteFactory.CreateFiltroCorte(input.Nome, "FiltroCorte", perfil, perfilSaída, poço.Trajetória, poço.ObterLitologiaPadrão(),
                    input.LimiteInferior, input.LimiteSuperior, input.TipoCorte, out var filtro);

                if (result.IsValid)
                {
                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo) filtro, "Filtros");
                    return CriarFiltroOutput.FiltroCriado(filtro.PerfisSaída.Perfis.First(), (Cálculo)filtro);
                }

                return CriarFiltroOutput.FiltroNãoCriado(string.Join(";\n",result.Errors));
            }
            catch (Exception e)
            {
                return CriarFiltroOutput.FiltroNãoCriado(e.Message);
            }
        }
    }
}
