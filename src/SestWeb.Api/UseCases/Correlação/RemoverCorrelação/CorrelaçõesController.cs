using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelação;

namespace SestWeb.Api.UseCases.Correlação.RemoverCorrelação
{
    /// <inheritdoc />
    [Route("api/correlacoes/delete")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IRemoverCorrelaçãoUseCase _removerCorrelaçãoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public CorrelaçõesController(IRemoverCorrelaçãoUseCase removerCorrelaçãoUseCase, Presenter presenter)
        {
            _removerCorrelaçãoUseCase = removerCorrelaçãoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove uma correlação.
        /// </summary>
        /// <param name="nome">Nome da correlação a ser removida.</param>
        /// <response code="204">Correlação removida com sucesso.</response>
        /// <response code="400">Não foi possível remover a correlação.</response>
        /// <response code="404">Correlação não encontrada.</response>
        [HttpDelete("{nome}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCorr(string nome)
        {
            var output = await _removerCorrelaçãoUseCase.Execute(nome);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
