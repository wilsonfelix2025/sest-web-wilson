using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.DuplicateFile;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.DuplicateFile
{
    ///<inheritdoc />
    [Route("api/pocoweb/files/duplicate")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IDuplicateFileUseCase _useCase;

        ///<inheritdoc />
        public FileController(Presenter presenter, IDuplicateFileUseCase useCase)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Duplica um arquivo.
        /// </summary>
        /// <param name="request">Objeto request contendo id do arquivo original.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DuplicateFile(DuplicateFileRequest request)
        {
            var output = await _useCase.Execute(Request.Headers["Authorization"].ToString(), request.Id);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
