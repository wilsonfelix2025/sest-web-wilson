using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.MoveFile;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.MoveFile
{
    [Route("api/pocoweb/files/move")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IMoveFileUseCase _useCase;

        public FileController(Presenter presenter, IMoveFileUseCase useCase)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Move um Arquivo para outro poço.
        /// </summary>
        /// <param name="request">Objeto request contendo o id do arquivo e o id do novo poço</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MoveFile(MoveFileRequest request)
        {
            var output = await _useCase.Execute(request.FileId, request.WellId);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
