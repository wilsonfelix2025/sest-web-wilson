using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarState;

namespace SestWeb.Api.UseCases.Poço.AtualizarState
{
    [Route("api/pocos/{id}/atualizar-state")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IAtualizarStateUseCase _atualizarStateUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IAtualizarStateUseCase atualizarStateUseCase, Presenter presenter)
        {
            _atualizarStateUseCase = atualizarStateUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Atualiza o state de um arquivo.
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
        public async Task<IActionResult> AtualizarState([FromRoute] string id, [FromBody] AtualizarStateInput input)
        {
            var output = await _atualizarStateUseCase.Execute(id, input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}