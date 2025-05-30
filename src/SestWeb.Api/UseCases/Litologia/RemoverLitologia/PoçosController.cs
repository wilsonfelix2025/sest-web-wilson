using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.RemoverLitologia;

namespace SestWeb.Api.UseCases.Litologia.RemoverLitologia
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/remover-litologia/{idLitologia}")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IRemoverLitologiaUseCase _removerLitologiaUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(IRemoverLitologiaUseCase removerLitologiaUseCase, Presenter presenter)
        {
            _removerLitologiaUseCase = removerLitologiaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove uma litologia.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="idLitologia">Id da litologia.</param>
        /// <response code="204">Litologia removida com sucesso.</response>
        /// <response code="400">Não foi possível remover a litologia.</response>
        /// <response code="404">Poço não encontrado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverLitologia([FromRoute] string id, [FromRoute] string idLitologia)
        {
            var output = await _removerLitologiaUseCase.Execute(id, idLitologia);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}