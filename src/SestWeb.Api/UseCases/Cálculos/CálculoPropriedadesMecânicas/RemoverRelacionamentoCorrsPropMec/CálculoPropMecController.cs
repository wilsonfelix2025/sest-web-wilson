using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMec
{
    /// <inheritdoc />
    [Route("api/propmec/remove-user-relationship")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly IRemoverRelacionamentoCorrsPropMecUseCase _removerRelacionamentoCorrsPropMecUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public CálculoPropMecController(IRemoverRelacionamentoCorrsPropMecUseCase removerRelacionamentoCorrsPropMecUseCase, Presenter presenter)
        {
            _removerRelacionamentoCorrsPropMecUseCase = removerRelacionamentoCorrsPropMecUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove um relacionamento de correlações de propriedades mecânicas do usuário.
        /// </summary>
        /// <param name="nome">Nome do relacionamento a ser removido.</param>
        /// <response code="204">Relacionamento removido com sucesso.</response>
        /// <response code="400">Não foi possível remover o relacionamento.</response>
        /// <response code="404">Relacionamento não encontrado.</response>
        [HttpDelete("{nome}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCorr(string nome)
        {
            var output = await _removerRelacionamentoCorrsPropMecUseCase.Execute(nome);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
