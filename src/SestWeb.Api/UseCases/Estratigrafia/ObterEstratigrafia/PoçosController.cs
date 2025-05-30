using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.ObterEstratigrafia;

namespace SestWeb.Api.UseCases.Estratigrafia.ObterEstratigrafia
{
    [Route("api/pocos/{id}/obter-estratigrafia")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterEstratigrafiaUseCase _obterEstratigrafiaUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterEstratigrafiaUseCase obterEstratigrafiaUseCase, Presenter presenter)
        {
            _obterEstratigrafiaUseCase = obterEstratigrafiaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém a estratigrafia de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Estratigrafia do poço.</returns>
        /// <response code="200">Estratigrafia do poço foi obtida.</response>
        /// <response code="400">Estratigrafia do poço não foi obtida.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterEstratigrafia([FromRoute]string id)
        {
            var output = await _obterEstratigrafiaUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}