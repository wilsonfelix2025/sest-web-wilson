using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologia;

namespace SestWeb.Api.UseCases.Litologia.ObterLitologia
{
    [Route("api/pocos/{id}/obter-litologia/{idLitologia}")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterLitologiaUseCase _oberLitologiaUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterLitologiaUseCase oberLitologiaUseCase, Presenter presenter)
        {
            _oberLitologiaUseCase = oberLitologiaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém uma litologia.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="idLitologia">Id da litologia.</param>
        /// <response code="200">A litologias foi obtida.</response>
        /// <response code="400">A litologia não foi obtida.</response>
        /// <response code="404">Não foi encontrada litologia com o id informado.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterLitologia([FromRoute] string id, [FromRoute] string idLitologia)
        {
            var output = await _oberLitologiaUseCase.Execute(id, idLitologia);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}