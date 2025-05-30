using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodosRelacionamentosCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterTodosRelacionamentosCorrsPropMec
{
    [Route("api/propmec/get-all-relationships")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly IObterTodosRelacionamentosCorrsPropMecUseCase _obterTodosRelacionamentosCorrsPropMecUseCase;
        private readonly Presenter _presenter;

        public CálculoPropMecController(IObterTodosRelacionamentosCorrsPropMecUseCase obterTodosRelacionamentosCorrsPropMecUseCase, Presenter presenter)
        {
            _obterTodosRelacionamentosCorrsPropMecUseCase = obterTodosRelacionamentosCorrsPropMecUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém todos os relacionamentos de correlações (usc,coesa,angat,restr).
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <response code="200">Retorna todos relacionamentos.</response>
        /// <response code="400">Não foi possível obter os relacionamentos.</response>
        /// <response code="404">Não foram encontrados relacionamentos.</response>
        [HttpGet("{idPoço}", Name = "get-all-relationships")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllRelationships(string idPoço)
        {
            var output = await _obterTodosRelacionamentosCorrsPropMecUseCase.Execute(idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
