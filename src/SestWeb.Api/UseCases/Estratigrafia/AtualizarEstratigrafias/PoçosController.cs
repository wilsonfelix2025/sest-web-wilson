using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.AtualizarEstratigrafias;

namespace SestWeb.Api.UseCases.Estratigrafia.AtualizarEstratigrafias
{
    [Route("api/pocos/{id}/atualizar-estratigrafias")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IAtualizarEstratigrafiasUseCase _atualizarEstratigrafiasUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IAtualizarEstratigrafiasUseCase atualizarEstratigrafiasUseCase, Presenter presenter)
        {
            _atualizarEstratigrafiasUseCase = atualizarEstratigrafiasUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Atualiza as estratigrafias de um poço.
        /// </summary>
        /// <param name="id">Id do poço a ser atualizado.</param>
        /// <param name="request">Dados necessários para atualizar a estratigrafia do poço.</param>
        /// <response code="200">A estratigrafia do poço foi atualizada com sucesso.</response>
        /// <response code="400">Não foi possível atualizar a estratigrafia do poço.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarEstratigrafias([FromRoute]string id, [FromBody] AtualizarEstratigrafiasRequest request)
        {
            var input = new AtualizarEstratigrafiasInput(request.ProfundidadeReferência, request.Estratigrafias);
            var output = await _atualizarEstratigrafiasUseCase.Execute(id, input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}