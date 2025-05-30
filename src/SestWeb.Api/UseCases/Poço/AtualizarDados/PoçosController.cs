using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados;

namespace SestWeb.Api.UseCases.Poço.AtualizarDados
{
    [Route("api/pocos/{id}/atualizar-dados")]
    [ApiController]
    public class PoçosController : ControllerBase
    {

        private readonly IAtualizarDadosUseCase _atualizarDadosUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IAtualizarDadosUseCase atualizarDadosUseCase, Presenter presenter)
        {
            _atualizarDadosUseCase = atualizarDadosUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Atualiza os dados de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="input">Dados de entrada.</param>
        /// <response code="200">Dados gerais atualizados com sucesso.</response>
        /// <response code="400">Não foi possível atualizar os dados gerais.</response>
        /// <response code="404">O poço não foi encontrado.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarDados([FromRoute] string id, [FromBody] AtualizarDadosInput input)
        {
            var output = await _atualizarDadosUseCase.Execute(id, input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
