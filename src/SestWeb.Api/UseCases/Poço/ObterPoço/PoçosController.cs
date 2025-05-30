using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.ObterPoço;

namespace SestWeb.Api.UseCases.Poço.ObterPoço
{
    [Route("api/pocos")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterPoçoUseCase _obterPoçoUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterPoçoUseCase obterPoçoUseCase, Presenter presenter)
        {
            _obterPoçoUseCase = obterPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obter um poço pelo id.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Um poço.</returns>
        /// <response code="200">Retorna o poço desejado.</response>
        /// <response code="400">Não foi possível obter o poço desejado.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpGet("{id}", Name = "ObterPoço")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPoço(string id)
        {
            var token = Request != null ? Request.Headers["Authorization"].ToString() : string.Empty;

            var output = await _obterPoçoUseCase.Execute(id, token);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}