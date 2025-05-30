using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorChaveUsuário;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelaçõesPorChaveUsuário
{
    [Route("api/correlacoes/get-by-user-key")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IObterCorrelaçõesPorChaveUsuárioUseCase _obterCorrelaçõesPorChaveUsuárioUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IObterCorrelaçõesPorChaveUsuárioUseCase obterCorrelaçõesPorChaveUsuárioUseCase, Presenter presenter)
        {
            _obterCorrelaçõesPorChaveUsuárioUseCase = obterCorrelaçõesPorChaveUsuárioUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém correlações pela chave do usuário criador.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <param name="userKey">Chave do usuário que criou a correlação.</param>
        /// <response code="200">Retorna as correlações que foram criadas pelo usuário com chave informada.</response>
        /// <response code="400">Não foi possível obter as correlações.</response>
        /// <response code="404">Não foram encontradas correlações para o mnemônico informado.</response>
        [HttpGet("{idPoço}/{userKey}", Name = "get-by-user-key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCorrsByUserKey(string idPoço, string userKey)
        {
            var output = await _obterCorrelaçõesPorChaveUsuárioUseCase.Execute(idPoço, userKey);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
