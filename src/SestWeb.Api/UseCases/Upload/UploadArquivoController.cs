using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Api.Helpers.Web;
using SestWeb.Application.UseCases.UploadUseCase;

namespace SestWeb.Api.UseCases.Upload
{
    [Route("api/upload")]
    [ApiController]
    public class UploadArquivoController : ControllerBase
    {
        private readonly IUploadArquivoUseCase _uploadArquivoUseCase;
        private readonly Presenter _presenter;

        public UploadArquivoController(IUploadArquivoUseCase uploadArquivoUseCase , Presenter presenter)
        {
            _uploadArquivoUseCase = uploadArquivoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Faz upload de um arquivo.
        /// </summary>
        /// <param name="request">Informações necessárias para o upload do arquivo.</param>
        /// <returns>O caminho do arquivo e o tamanho em bytes.</returns>
        /// <response code="200">Arquivo importado com sucesso.</response>
        /// <response code="400">O arquivo não foi importado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadArquivo([FromForm] UploadArquivoRequest request)
        {
            var extensao = Path.GetExtension(request.Arquivo.FileName).ToLowerInvariant();
            var formFileContent = await FileHelpers.ProcessFormFile(request.Arquivo);

            var output = await _uploadArquivoUseCase.Execute(extensao, formFileContent);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
