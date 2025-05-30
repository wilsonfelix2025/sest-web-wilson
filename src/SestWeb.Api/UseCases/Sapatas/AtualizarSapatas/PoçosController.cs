using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.SapataUseCases.AtualizarSapatas;


namespace SestWeb.Api.UseCases.Sapatas.AtualizarSapatas
{
    [Route("api/pocos/{id}/atualizar-sapatas")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IAtualizarSapatasUseCase _atualizarSapatasUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IAtualizarSapatasUseCase atualizarSapatasUseCase, Presenter presenter)
        {
            _atualizarSapatasUseCase = atualizarSapatasUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Atualiza as sapatas de um poço.
        /// </summary>
        /// <param name="id">Id do poço a ser atualizado.</param>
        /// <param name="request">Dados necessários para atualizar a lista de sapatas do poço.</param>
        /// <response code="200">A lista de sapatas do poço foi atualizada com sucesso.</response>
        /// <response code="400">Não foi possível atualizar a lista de sapatas do poço.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarSapatas([FromRoute]string id, [FromBody] AtualizarSapatasRequest request)
        {
            var input = new AtualizarSapatasInput(request.ProfundidadeReferência, request.Sapatas);
            var output = await _atualizarSapatasUseCase.Execute(id, input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
