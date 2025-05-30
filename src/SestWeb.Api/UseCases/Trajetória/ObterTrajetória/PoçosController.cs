using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrajetóriaUseCases.ObterTrajetória;

namespace SestWeb.Api.UseCases.Trajetória.ObterTrajetória
{
    [Route("api/pocos/{id}/obter-trajetoria")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterTrajetóriaUseCase _obterTrajetóriaUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterTrajetóriaUseCase obterTrajetóriaUseCase, Presenter presenter)
        {
            _obterTrajetóriaUseCase = obterTrajetóriaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém a trajetória de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Trajetória do poço.</returns>
        /// <response code="200">Trajetória do poço foi obtida.</response>
        /// <response code="400">Trajetória do poço não foi obtida.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterTrajetória([FromRoute]string id)
        {
            var output = await _obterTrajetóriaUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}