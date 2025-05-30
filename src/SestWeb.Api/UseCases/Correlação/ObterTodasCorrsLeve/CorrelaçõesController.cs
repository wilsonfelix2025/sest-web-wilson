using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrsLeve;

namespace SestWeb.Api.UseCases.Correlação.ObterTodasCorrsLeve
{
    [Route("api/correlacoes/get-all-slim")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IObterTodasCorrsLeveUseCase _obterTodasCorrsLeveUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IObterTodasCorrsLeveUseCase obterTodasCorrsLeveUseCase, Presenter presenter)
        {
            _obterTodasCorrsLeveUseCase = obterTodasCorrsLeveUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém todas as correlações. Retorna somente nome, perfilSaída e chaveAutor de cada correlação. 
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <response code="200">Retorna todas correlações.</response>
        /// <response code="400">Não foi possível obter as correlações.</response>
        /// <response code="404">Não foram encontradas correlações.</response>
        [HttpGet("{idPoço}", Name = "get-all-slim")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LightweightGetAllCorrs(string idPoço)
        {
            var output = await _obterTodasCorrsLeveUseCase.Execute(idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
