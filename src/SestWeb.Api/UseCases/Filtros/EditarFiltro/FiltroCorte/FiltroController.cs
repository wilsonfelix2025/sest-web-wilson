using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroCorte;
using SestWeb.Domain.Entities.Cálculos.Filtros;

namespace SestWeb.Api.UseCases.Filtros.EditarFiltro.FiltroCorte
{
    [Route("api/editar-filtro-corte")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly IEditarFiltroCorteUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(IEditarFiltroCorteUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarFiltro(FiltroCorteRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarFiltroCorteInput PreencherInput(FiltroCorteRequest request)
        {
            var input = new EditarFiltroCorteInput
            {
                LimiteInferior = request.LimiteInferior,
                LimiteSuperior = request.LimiteSuperior,
                IdPerfil = request.IdPerfil,
                TipoCorte = request.TipoCorte,
                TipoFiltro = TipoFiltroEnum.FiltroCorte,
                Nome = request.Nome
            };

            return input;
        }

    }
}
