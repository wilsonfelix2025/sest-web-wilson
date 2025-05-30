using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.ObterPoços;

namespace SestWeb.Api.UseCases.Poço.ObterPoços
{
    [Route("api/pocos")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterPoçosUseCase _obterPoçosUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterPoçosUseCase obterPoçosUseCase, Presenter presenter)
        {
            _obterPoçosUseCase = obterPoçosUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obter poços.
        /// </summary>
        /// <response code="200">Retorna coleção com os poços.</response>
        [HttpGet(Name = "ObterPoços")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPoços()
        {
            var output = await _obterPoçosUseCase.Execute();
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
