using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase.LerArquivoPoçoWebUseCase;

namespace SestWeb.Api.UseCases.Importação.ImportarPoçoWeb.LerArquivoPoçoWeb
{
    [Route("api/[controller]")]
    [ApiController]
    public class LerArquivoPoçoWebController : ControllerBase
    {
        private readonly ILerArquivoPoçoWebUseCase _lerArquivoUseCase;
        private readonly LerArquivoPoçoWebPresenter _presenter;

        public LerArquivoPoçoWebController(ILerArquivoPoçoWebUseCase lerArquivoUseCase,
            LerArquivoPoçoWebPresenter presenter)
        {
            _lerArquivoUseCase = lerArquivoUseCase;
            _presenter = presenter;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LerArquivoPoçoWeb(string urlArquivo)
        {
            Log.Information("Recebendo leitura do arquivo poçoweb: " + urlArquivo);

            var token = Request.Headers["Authorization"].ToString();
            var output = await _lerArquivoUseCase.Execute(urlArquivo, token);

            Log.Information("Finalizando leitura do arquivo poçoweb: " + urlArquivo + " Output:" + output);

            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

    }
}
