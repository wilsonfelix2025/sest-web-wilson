using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.ObterDadosEntrada;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.ObterDadosEntrada
{
    [Route("api/obter-dados-entrada-calculo")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IObterDadosEntradaCálculoPerfisUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IObterDadosEntradaCálculoPerfisUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterDadosEntradaCálculoPerfis(string idCálculo, string idPoço)
        {
            var output = await _useCase.Execute(idCálculo, idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
