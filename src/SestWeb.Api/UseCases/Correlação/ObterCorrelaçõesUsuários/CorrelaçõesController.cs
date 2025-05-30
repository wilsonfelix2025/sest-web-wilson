using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesUsuários;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelaçõesUsuários
{
    [Route("api/correlacoes/get-user-corrs")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IObterCorrelaçõesUsuáriosUseCase _obterCorrelaçõesUsuáriosUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IObterCorrelaçõesUsuáriosUseCase obterCorrelaçõesUsuáriosUseCase, Presenter presenter)
        {
            _obterCorrelaçõesUsuáriosUseCase = obterCorrelaçõesUsuáriosUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém todas as correlações criadas por usuários.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <response code="200">Retorna todas .</response>
        /// <response code="400">Não foi possível obter as correlações.</response>
        /// <response code="404">Não foram encontradas correlações de usuários.</response>
        [HttpGet("{idPoço}", Name = "get-user-corrs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserCorrs(string idPoço)
        {
            var output = await _obterCorrelaçõesUsuáriosUseCase.Execute(idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
