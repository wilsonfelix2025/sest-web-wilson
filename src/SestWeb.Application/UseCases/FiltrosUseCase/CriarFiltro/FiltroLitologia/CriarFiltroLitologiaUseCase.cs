using SestWeb.Application.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.Factory;
using SestWeb.Domain.Entities.Perfis.Factory;

namespace SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroLitologia
{
    internal class CriarFiltroLitologiaUseCase : ICriarFiltroLitologiaUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IFiltroLitologiaFactory _filtroLitologiaFactory;

        public CriarFiltroLitologiaUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IFiltroLitologiaFactory filtroLitologiaFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _filtroLitologiaFactory = filtroLitologiaFactory;
        }

        public async Task<CriarFiltroOutput> Execute(CriarFiltroLitologiaInput input)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfil);
                if (perfil == null)
                    return CriarFiltroOutput.FiltroNãoCriado("Filtro não pode ser criado. Perfil não encontrado");

                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfil.IdPoço);

                var perfilSaída = PerfisFactory.Create(perfil.Mnemonico, input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());

                var result = _filtroLitologiaFactory.CreateFiltroLitologia(input.Nome, "FiltroLitologia", perfil, perfilSaída, poço.Trajetória, null,
                    input.LimiteInferior, input.LimiteSuperior, input.TipoCorte, input.LitologiasSelecionadas, out var filtro);

                if (result.IsValid)
                {
                    filtro.Execute();
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
