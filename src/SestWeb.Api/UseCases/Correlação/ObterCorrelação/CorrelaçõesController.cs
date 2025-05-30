using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçãoPeloNome;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelação
{
    [Route("api/correlacoes/get-by-name")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IObterCorrelaçãoPeloNomeUseCase _obterCorrelaçãoUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IObterCorrelaçãoPeloNomeUseCase obterCorrelaçãoUseCase, Presenter presenter)
        {
            _obterCorrelaçãoUseCase = obterCorrelaçãoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém uma correlação pelo nome.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <param name="name">Nome da correlação.</param>
        /// <response code="200">Retorna a correlação desejada.</response>
        /// <response code="400">Não foi possível obter a correlação desejada.</response>
        /// <response code="404">Não foi encontrada correlação com o nome informado.</response>
        [HttpGet("{idPoço}/{name}", Name = "get-by-name")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCorrByName(string idPoço, string name)
        {
            var output = await _obterCorrelaçãoUseCase.Execute(idPoço, name);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
