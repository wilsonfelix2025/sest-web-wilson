using SestWeb.Application.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.Factory;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroLitologia
{
    internal class EditarFiltroLitologiaUseCase : IEditarFiltroLitologiaUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IFiltroLitologiaFactory _filtroLitologiaFactory;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarFiltroLitologiaUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IFiltroLitologiaFactory filtroLitologiaFactory, IPipelineUseCase pipelineUseCase)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _filtroLitologiaFactory = filtroLitologiaFactory;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<EditarFiltroOutput> Execute(EditarFiltroLitologiaInput input)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfil);
                if (perfil == null)
                    return EditarFiltroOutput.FiltroNãoEditado("Filtro não pode ser editado. Perfil não encontrado");

                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfil.IdPoço);

                var perfilSaída = PerfisFactory.Create(perfil.Mnemonico, input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());
                perfilSaída.EditarId(perfil.Id);
                perfilSaída.EditarEstiloVisual(perfil.EstiloVisual);

                if (perfil.Trend != null)
                    perfilSaída.Trend = perfil.Trend;

                var idPerfilEntrada = poço.Cálculos.First(f => f.Id.ToString() == perfil.IdCálculo).PerfisEntrada.IdPerfis.First();
                var perfilEntrada = await _perfilReadOnlyRepository.ObterPerfil(idPerfilEntrada);
                
                var result = _filtroLitologiaFactory.CreateFiltroLitologia(input.Nome, "FiltroLitologia", perfilEntrada, perfilSaída, poço.Trajetória, null,
                    input.LimiteInferior, input.LimiteSuperior, input.TipoCorte, input.LitologiasSelecionadas, out var filtro);

                if (result.IsValid)
                {
                    filtro.Execute();

                    await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo)filtro, perfil.IdCálculo, "Filtros");

                    var perfisAlterados = await _pipeUseCase.Execute(poço, filtro, perfil.IdCálculo);

                    return EditarFiltroOutput.FiltroEditado(filtro.PerfisSaída.Perfis.First(), (Cálculo)filtro, perfisAlterados);
                }

                return EditarFiltroOutput.FiltroNãoEditado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return EditarFiltroOutput.FiltroNãoEditado(e.Message);
            }
        }
    }
}
