using System;
using System.Linq;
using SestWeb.Application.Repositories;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros.Simples.Factory;
using SestWeb.Domain.Entities.Perfis.Factory;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroSimples
{
    internal class CriarFiltroSimplesUseCase : ICriarFiltroSimplesUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IFiltroSimplesFactory _filtroSimplesFactory;

        public CriarFiltroSimplesUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IFiltroSimplesFactory filtroSimplesFactory
        )
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _filtroSimplesFactory = filtroSimplesFactory;
        }

        public async Task<CriarFiltroOutput> Execute(CriarFiltroSimplesInput input)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfil);
                if (perfil == null)
                    return CriarFiltroOutput.FiltroNãoCriado("Filtro não pode ser criado. Perfil não encontrado");

                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfil.IdPoço);
                var perfilSaída = PerfisFactory.Create(perfil.Mnemonico, input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());

                var result = _filtroSimplesFactory.CreateFiltroSimples(input.Nome, "FiltroSimples", perfil, perfilSaída, poço.Trajetória, null,
                    input.LimiteInferior, input.LimiteSuperior, input.TipoCorte, input.DesvioMáximo, out var filtro);

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
