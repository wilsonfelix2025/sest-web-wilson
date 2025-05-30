using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.RemoverPoço;

namespace SestWeb.Api.UseCases.Poço.RemoverPoço
{
    /// <inheritdoc />
    [Route("api/pocos")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IRemoverPoçoUseCase _removerPoçoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(IRemoverPoçoUseCase removerPoçoUseCase, Presenter presenter)
        {
            _removerPoçoUseCase = removerPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <response code="204">Poço removido com sucesso.</response>
        /// <response code="400">Não foi possível remover o poço.</response>
        /// <response code="404">Poço não encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverPoço(string id)
        {
            var output = await _removerPoçoUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}