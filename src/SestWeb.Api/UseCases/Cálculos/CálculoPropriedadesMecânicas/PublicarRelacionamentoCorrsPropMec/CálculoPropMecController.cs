using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.PublicarRelacionamentoCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.PublicarRelacionamentoCorrsPropMec
{
    [Route("api/propmec/{idPoço}/{nome}/publish-relationship")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly IPublicarRelacionamentoCorrsPropMecUseCase _publicarRelacionamentoCorrsPropMecUseCase;
        private readonly Presenter _presenter;

        public CálculoPropMecController(IPublicarRelacionamentoCorrsPropMecUseCase publicarRelacionamentoCorrsPropMecUseCase, Presenter presenter)
        {
            _publicarRelacionamentoCorrsPropMecUseCase = publicarRelacionamentoCorrsPropMecUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Publica um relacionamento exclusivo do caso do poço para se tornar disponível a todos usuários.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <param name="nome">Nome do relacionamento.</param>
        /// <response code="200">Relacionamento publicado com sucesso.</response>
        /// <response code="400">Não foi possível puublicar o relacionamento.</response>
        /// <response code="404">O relacionamento não foi encontrado.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PublishCorr([FromRoute] string idPoço, [FromRoute] string nome)
        {
            var output = await _publicarRelacionamentoCorrsPropMecUseCase.Execute(idPoço, nome);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
