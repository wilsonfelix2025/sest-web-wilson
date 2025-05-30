using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMecPoço;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMecPoço
{
    /// <inheritdoc />
    [Route("api/propmec/remove-well-relationship")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly IRemoverRelacionamentoCorrsPropMecUseCasePoço _removerRelacionamentoCorrsPropMecUseCasePoço;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public CálculoPropMecController(IRemoverRelacionamentoCorrsPropMecUseCasePoço removerRelacionamentoCorrsPropMecUseCasePoço, Presenter presenter)
        {
            _removerRelacionamentoCorrsPropMecUseCasePoço = removerRelacionamentoCorrsPropMecUseCasePoço;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove uma correlação exclusiva do caso de uso do poço.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <param name="nome">Nome da correlação a ser removida.</param>
        /// <response code="204">Correlação removida com sucesso.</response>
        /// <response code="400">Não foi possível remover a correlação.</response>
        /// <response code="404">Correlação não encontrada.</response>
        [HttpDelete("{idPoço}/{nome}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWellRelationship(string idPoço, string nome)
        {
            var output = await _removerRelacionamentoCorrsPropMecUseCasePoço.Execute(idPoço, nome);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
