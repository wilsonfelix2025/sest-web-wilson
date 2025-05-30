using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ObjetivoUseCases.AtualizarObjetivos;

namespace SestWeb.Api.UseCases.Objetivos.AtualizarObjetivos
{
    [Route("api/pocos/{id}/atualizar-objetivos")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IAtualizarObjetivosUseCase _atualizarSapatasUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IAtualizarObjetivosUseCase atualizarSapatasUseCase, Presenter presenter)
        {
            _atualizarSapatasUseCase = atualizarSapatasUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Atualiza os objetivos de um poço.
        /// </summary>
        /// <param name="id">Id do poço a ser atualizado.</param>
        /// <param name="request">Dados necessários para atualizar os objetivos do poço.</param>
        /// <response code="200">A lista de objetivos do poço foi atualizada com sucesso.</response>
        /// <response code="400">Não foi possível atualizar a lista de objetivos do poço.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarObjetivos([FromRoute]string id, [FromBody] AtualizarObjetivosRequest request)
        {
            var input = new AtualizarObjetivosInput(request.ProfundidadeReferência, request.Objetivos);
            var output = await _atualizarSapatasUseCase.Execute(id, input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
