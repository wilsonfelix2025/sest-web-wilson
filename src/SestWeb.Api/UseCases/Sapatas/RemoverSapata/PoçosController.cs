using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.SapataUseCases.RemoverSapata;

namespace SestWeb.Api.UseCases.Sapatas.RemoverSapata
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/remover-sapata")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IRemoverSapataUseCase _removerSapataUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(IRemoverSapataUseCase removerSapataUseCase, Presenter presenter)
        {
            _removerSapataUseCase = removerSapataUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove uma sapata.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Profundidade medida da sapata.</param>
        /// <response code="204">Sapata removida com sucesso.</response>
        /// <response code="400">Não foi possível remover a sapata.</response>
        /// <response code="404">Poço não encontrado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverSapata([FromRoute] string id, RemoverSapataRequest request)
        {
            var output = await _removerSapataUseCase.Execute(id, request.ProfundidadeMedida);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}