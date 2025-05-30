using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ObjetivoUseCases.RemoverObjetivo;

namespace SestWeb.Api.UseCases.Objetivos.RemoverObjetivo
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/remover-objetivo")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IRemoverObjetivoUseCase _removerObjetivoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(IRemoverObjetivoUseCase removerObjetivoUseCase, Presenter presenter)
        {
            _removerObjetivoUseCase = removerObjetivoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove um objetivo.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Informações necessárias para a remoção do objetivo.</param>
        /// <response code="204">Objetivo removido com sucesso.</response>
        /// <response code="400">Não foi possível remover o objetivo.</response>
        /// <response code="404">Poço não encontrado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverObjetivo([FromRoute] string id, RemoverObjetivoRequest request)
        {
            var output = await _removerObjetivoUseCase.Execute(id, request.ProfundidadeMedida);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}