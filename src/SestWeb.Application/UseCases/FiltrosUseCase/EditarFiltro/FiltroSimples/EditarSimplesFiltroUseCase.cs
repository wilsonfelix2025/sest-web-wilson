using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros.Simples.Factory;
using SestWeb.Domain.Entities.Perfis.Factory;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroSimples
{
    internal class EditarFiltroSimplesUseCase : IEditarFiltroSimplesUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IFiltroSimplesFactory _filtroSimplesFactory;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarFiltroSimplesUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IFiltroSimplesFactory filtroSimplesFactory, IPipelineUseCase pipelineUseCase
           )
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _filtroSimplesFactory = filtroSimplesFactory;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<EditarFiltroOutput> Execute(EditarFiltroSimplesInput input)
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

                var idPerfilEntrada = poço.Cálculos.First(f => f.Id.ToString() == perfil.IdCálculo).PerfisEntrada.IdPerfis.First();
                var perfilEntrada = await _perfilReadOnlyRepository.ObterPerfil(idPerfilEntrada);

                var result = _filtroSimplesFactory.CreateFiltroSimples(input.Nome, "FiltroSimples", perfilEntrada, perfilSaída, poço.Trajetória, null,
                    input.LimiteInferior, input.LimiteSuperior, input.TipoCorte, input.DesvioMáximo, out var filtro);

                if (result.IsValid)
                {
                    var outputPerfil = filtro.PerfisSaída.Perfis.First();
                    await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo)filtro, perfil.IdCálculo, "Filtros");

                    var perfisAlterados = await _pipeUseCase.Execute(poço, filtro, perfil.IdCálculo);

                    return EditarFiltroOutput.FiltroEditado(outputPerfil, (Cálculo)filtro, perfisAlterados);
                }

                return EditarFiltroOutput.FiltroNãoEditado(string.Join(";\n",result.Errors));
            }
            catch (Exception e)
            {
                return EditarFiltroOutput.FiltroNãoEditado(e.Message);
            }
        }
    }
}
