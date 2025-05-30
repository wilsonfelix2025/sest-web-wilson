using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.RenameFile;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.RenameFile
{
    [Route("api/pocoweb/files/rename")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IRenameFileUseCase _useCase;

        public FileController(Presenter presenter, IRenameFileUseCase useCase)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Renomeia um Arquivo.
        /// </summary>
        /// <param name="request">Objeto request contendo o id do arquivo e o novo nome.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RenameFile(RenameFileRequest request)
        {
            var output = await _useCase.Execute(request.Id, request.NewName);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
