using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.CreateFile;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.CreateFile
{
    ///<inheritdoc />
    [Route("api/pocoweb/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly ICreateFileUseCase _useCase;

        ///<inheritdoc />
        public FileController(Presenter presenter, ICreateFileUseCase useCase)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Cria um novo arquivo.
        /// </summary>
        /// <param name="request">Objeto request contendo informações do arquivo</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFile(FileRequest request)
        {
            CreateFileOutput output = await _useCase.Execute(Request.Headers["Authorization"].ToString(), request.Name, request.WellId, request.Description, request.FileType);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
