using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologias;

namespace SestWeb.Api.UseCases.Litologia.ObterLitologias
{
    [Route("api/pocos/{id}/obter-litologias")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterLitologiasUseCase _obterLitologiasUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterLitologiasUseCase obterLitologiasUseCase, Presenter presenter)
        {
            _obterLitologiasUseCase = obterLitologiasUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém as litologias de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <response code="200">As litologias do poço foram obtidas.</response>
        /// <response code="400">As litologias do poço não foram obtidos.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterLitologias([FromRoute]string id)
        {
            var output = await _obterLitologiasUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}