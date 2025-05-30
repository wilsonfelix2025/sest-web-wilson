using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroSimples;
using SestWeb.Domain.Entities.Cálculos.Filtros;

namespace SestWeb.Api.UseCases.Filtros.EditarFiltro.FiltroSimples
{
    [Route("api/editar-filtro-simples")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly IEditarFiltroSimplesUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(IEditarFiltroSimplesUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarFiltro(FiltroSimplesRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarFiltroSimplesInput PreencherInput(FiltroSimplesRequest request)
        {
            var input = new EditarFiltroSimplesInput
            {
                DesvioMáximo = request.DesvioMáximo,
                LimiteInferior = request.LimiteInferior,
                LimiteSuperior = request.LimiteSuperior,
                IdPerfil = request.IdPerfil,
                TipoCorte = request.TipoCorte,
                TipoFiltro = TipoFiltroEnum.FiltroSimples,
                Nome = request.Nome
            };

            return input;
        }

    }
}
