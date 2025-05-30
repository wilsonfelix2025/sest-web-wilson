using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.ObterCálculo;

namespace SestWeb.Api.UseCases.Cálculos.ObterCálculo
{
    [Route("api/obter-calculo")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IObterCálculoUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IObterCálculoUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterCálculo(string idCálculo, string idPoço)
        {
            var output = await _useCase.Execute(idCálculo, idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
