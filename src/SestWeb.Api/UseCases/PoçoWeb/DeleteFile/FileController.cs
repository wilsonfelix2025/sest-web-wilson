using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.DeleteFile;

namespace SestWeb.Api.UseCases.PoçoWeb.DeleteFile
{
    /// <inheritdoc />
    [Route("api/pocoweb/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IDeleteFileUseCase _useCase;

        /// <inheritdoc />
        public FileController(Presenter presenter, IDeleteFileUseCase useCase)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Remove um arquivo.
        /// </summary>
        /// <param name="id">Id do arquivo.</param>
        /// <response code="204">Arquivo removido com sucesso.</response>
        /// <response code="400">Não foi possível remover o arquivo.</response>
        /// <response code="404">Arquivo não encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFile(string id)
        {
            var output = await _useCase.Execute(Request.Headers["Authorization"].ToString(), id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}