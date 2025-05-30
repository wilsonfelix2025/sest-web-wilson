using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodasCorrelaçõesPossíveisPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterTodasCorrelaçõesPossíveisPropMec
{
    [Route("api/propmec/get-all-correlations")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly IObterTodasCorrelaçõesPossíveisPropMecUseCase _obterTodasCorrelaçõesPossíveisPropMecUseCase;
        private readonly Presenter _presenter;

        public CálculoPropMecController(IObterTodasCorrelaçõesPossíveisPropMecUseCase obterTodasCorrelaçõesPossíveisPropMecUseCase, Presenter presenter)
        {
            _obterTodasCorrelaçõesPossíveisPropMecUseCase = obterTodasCorrelaçõesPossíveisPropMecUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém todas correlações ucs, coesa, angat e restr que estão em relacionamentos, agrupadas por seus grupos Litológicos.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <response code="200">Retorna as correlações ucs, coesa, angat e restr que estão em relacionamentos do grupoLitológico informado.</response>
        /// <response code="400">Não foi possível obter as correlações.</response>
        /// <response code="404">Não foram encontradas correlações para o grupo litológico informado.</response>
        [HttpGet("{idPoço}", Name = "get-all-corrspropmec")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllPropMecCorrs(string idPoço)
        {
            var output = await _obterTodasCorrelaçõesPossíveisPropMecUseCase.Execute(idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
