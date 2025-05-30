using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrelações;

namespace SestWeb.Api.UseCases.Correlação.ObterTodasCorrelações
{
    [Route("api/correlacoes/get-all")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IObterTodasCorrelaçõesUseCase _obterTodasCorrelaçõesUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IObterTodasCorrelaçõesUseCase obterTodasCorrelaçõesUseCase, Presenter presenter)
        {
            _obterTodasCorrelaçõesUseCase = obterTodasCorrelaçõesUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém todas as correlações.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <response code="200">Retorna todas correlações.</response>
        /// <response code="400">Não foi possível obter as correlações.</response>
        /// <response code="404">Não foram encontradas correlações.</response>
        [HttpGet("{idPoço}", Name = "get-all-corrs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCorrs(string idPoço)
        {
            var output = await _obterTodasCorrelaçõesUseCase.Execute(idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
