using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterAutoresCorrelações;

namespace SestWeb.Api.UseCases.Correlação.ObterAutores
{
    [Route("api/correlacoes/get-all-authors")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IObterAutoresCorrelaçõesUseCase _obterAutoresCorrelaçõesUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IObterAutoresCorrelaçõesUseCase obterAutoresCorrelaçõesUseCase, Presenter presenter)
        {
            _obterAutoresCorrelaçõesUseCase = obterAutoresCorrelaçõesUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém todos autores criadores de correlações.
        /// </summary>
        /// <response code="200">Retorna todas autores de correlações.</response>
        /// <response code="400">Não foi possível obter os autores de correlações.</response>
        /// <response code="404">Não foram encontradas autores de correlações.</response>
        [HttpGet("{idPoço}", Name = "get-all-authors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAuthors(string idPoço)
        {
            var output = await _obterAutoresCorrelaçõesUseCase.Execute(idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
