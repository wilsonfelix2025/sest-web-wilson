using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPorTipoPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterCorrelaçõesPorTipoPropMec
{
    [Route("api/propmec/get-corrs-by-mnemonic")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly IObterCorrelaçõesPorTipoPropMecUseCase _obterCorrelaçõesPorTipoPropMecUseCase;
        private readonly Presenter _presenter;

        public CálculoPropMecController(IObterCorrelaçõesPorTipoPropMecUseCase obterCorrelaçõesPorTipoPropMecUseCase, Presenter presenter)
        {
            _obterCorrelaçõesPorTipoPropMecUseCase = obterCorrelaçõesPorTipoPropMecUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém todas correlações do mnemônico informado (ucs ou coesa ou angat ou restr) que estão em relacionamentos do grupoLitológico informado.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <param name="litoGroup">Grupo litológico.</param>
        /// <param name="mnemonic">UCS ou COESA ou ANGAT ou RESTR</param>
        /// <response code="200">Retorna as correlações do mnemônico informado que estão em relacionamentos do grupoLitológico informado.</response>
        /// <response code="400">Não foi possível obter as correlações.</response>
        /// <response code="404">Não foram encontradas correlações para o mnemônico e grupo litológico informado.</response>
        [HttpGet("{idPoço}/{litoGroup}/{mnemonic}", Name = "get-propmeccorrs-by-mnemonic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCorrsByMnemonic(string idPoço, string litoGroup, string mnemonic)
        {
            var output = await _obterCorrelaçõesPorTipoPropMecUseCase.Execute(idPoço, litoGroup, mnemonic);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
