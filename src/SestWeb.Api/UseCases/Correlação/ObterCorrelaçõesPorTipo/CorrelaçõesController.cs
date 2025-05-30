using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorTipo;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelaçõesPorTipo
{
    [Route("api/correlacoes/get-by-mnemonic")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IObterCorrelaçõesPorTipoUseCase _obterCorrelaçõesPorTipoUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IObterCorrelaçõesPorTipoUseCase obterCorrelaçõesPorTipoUseCase, Presenter presenter)
        {
            _obterCorrelaçõesPorTipoUseCase = obterCorrelaçõesPorTipoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém todas correlações que possuem perfil de saída com o mnemônico informado.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <param name="mnemonic">Mnemônico do perfil de saída da correlação.</param>
        /// <response code="200">Retorna as correlações que possuem perfil de saída com mnemônico informado.</response>
        /// <response code="400">Não foi possível obter as correlações.</response>
        /// <response code="404">Não foram encontradas correlações para o mnemônico informado.</response>
        [HttpGet("{idPoço}/{mnemonic}", Name = "get-corrs-by-mnemonic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCorrsByMnemonic(string idPoço, string mnemonic)
        {
            var output = await _obterCorrelaçõesPorTipoUseCase.Execute(idPoço, mnemonic);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
