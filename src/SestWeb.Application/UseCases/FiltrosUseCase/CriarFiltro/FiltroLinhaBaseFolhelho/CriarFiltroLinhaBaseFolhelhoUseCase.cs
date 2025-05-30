using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.Factory;
using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Factory;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroLinhaBaseFolhelho
{
    internal class CriarFiltroLinhaBaseFolhelhoUseCase : ICriarFiltroLinhaBaseFolhelhoUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IFiltroLinhaBaseFolhelhoFactory _filtroLinhaBaseFolhelhoFactory;

        public CriarFiltroLinhaBaseFolhelhoUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IFiltroLinhaBaseFolhelhoFactory filtroLinhaBaseFolhelhoFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _filtroLinhaBaseFolhelhoFactory = filtroLinhaBaseFolhelhoFactory;
        }

        public async Task<CriarFiltroOutput> Execute(CriarFiltroLinhaBaseFolhelhoInput input)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfil);
                if (perfil == null)
                    return CriarFiltroOutput.FiltroNãoCriado("Filtro não pode ser criado. Perfil não encontrado");

                var perfilLBF = await _perfilReadOnlyRepository.ObterPerfil(input.IdLBF);
                
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfil.IdPoço);

                var perfilSaída = PerfisFactory.Create(perfil.Mnemonico, input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());

                var result = _filtroLinhaBaseFolhelhoFactory.CreateFiltroLinhaBaseFolhelho(input.Nome, "FiltroLinhaBaseFolhelho", perfil, perfilSaída, poço.Trajetória, null,
                    input.LimiteInferior, input.LimiteSuperior, input.TipoCorte, perfilLBF, out var filtro);

                if (result.IsValid)
                {
                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)filtro, "Filtros");
                    return CriarFiltroOutput.FiltroCriado(filtro.PerfisSaída.Perfis.First(), (Cálculo)filtro);
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
