
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroLinhaBaseFolhelho;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Filtros.EditarFiltro.FiltroLinhaBaseFolhelho
{
    [Route("api/editar-filtro-lbf")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly IEditarFiltroLinhaBaseFolhelhoUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(IEditarFiltroLinhaBaseFolhelhoUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarFiltro(FiltroLinhaBaseFolhelhoRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarFiltroLinhaBaseFolhelhoInput PreencherInput(FiltroLinhaBaseFolhelhoRequest request)
        {
            var input = new EditarFiltroLinhaBaseFolhelhoInput
            {
                IdLBF = request.IdLBF,
                LimiteInferior = request.LimiteInferior,
                LimiteSuperior = request.LimiteSuperior,
                IdPerfil = request.IdPerfil,
                TipoCorte = request.TipoCorte,
                TipoFiltro = TipoFiltroEnum.FiltroLinhaBaseFolhelho,
                Nome = request.Nome
            };

            return input;
        }
    }
}
