using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais;

namespace SestWeb.Api.UseCases.Poço.AtualizarDadosGerais
{
    [Route("api/pocos/{id}/atualizar-dados-gerais")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IAtualizarDadosGeraisUseCase _atualizarDadosGeraisUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IAtualizarDadosGeraisUseCase atualizarDadosGeraisUseCase, Presenter presenter)
        {
            _atualizarDadosGeraisUseCase = atualizarDadosGeraisUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Atualiza os dados gerais de um poço.
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
        public async Task<IActionResult> AtualizarDadosGerais([FromRoute] string id, [FromBody] AtualizarDadosGeraisInput input)
        {
            var output = await _atualizarDadosGeraisUseCase.Execute(id, input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}