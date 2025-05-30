using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetFiles;

namespace SestWeb.Api.UseCases.PoçoWeb.GetFiles
{
    [Route("api/pocoweb/files/get-files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IGetFilesUseCase _useCase;
        private readonly Presenter _presenter;

        public FileController(IGetFilesUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFiles(string tipoArquivo)
        {

            var token = Request == null ? string.Empty : Request.Headers["Authorization"].ToString();
            var output = await _useCase.Execute(token, tipoArquivo);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
