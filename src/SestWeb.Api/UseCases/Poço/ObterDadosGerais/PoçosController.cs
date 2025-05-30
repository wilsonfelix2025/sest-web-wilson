using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.ObterDadosGerais;

namespace SestWeb.Api.UseCases.Poço.ObterDadosGerais
{
    [Route("api/pocos/{id}/obter-dados-gerais")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterDadosGeraisUseCase _obterDadosGeraisUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterDadosGeraisUseCase obterDadosGeraisUseCase, Presenter presenter)
        {
            _obterDadosGeraisUseCase = obterDadosGeraisUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém os dados gerais de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Os dados gerais do poço.</returns>
        /// <response code="200">Dados gerais do poço foram obtidos.</response>
        /// <response code="400">Dados gerais do poço não puderam ser obtidos.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterDadosGerais([FromRoute]string id)
        {
            var output = await _obterDadosGeraisUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}