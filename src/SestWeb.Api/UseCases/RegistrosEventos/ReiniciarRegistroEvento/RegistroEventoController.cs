using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.RegistrosEventosUseCases.ReiniciarRegistroEvento;

namespace SestWeb.Api.UseCases.RegistrosEventos.ReiniciarRegistroEvento
{
    /// <inheritdoc />
    [Route("api/registrosEventos")]
    [ApiController]
    public class RegistrosEventosController : ControllerBase
    {
        private readonly IReiniciarRegistroEventoUseCase _reiniciarRegistroEventoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public RegistrosEventosController(IReiniciarRegistroEventoUseCase reiniciarRegistroEventoUseCase, Presenter presenter)
        {
            _reiniciarRegistroEventoUseCase = reiniciarRegistroEventoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Reinicia um registro / evento, removendo os trechos salvos.
        /// </summary>
        /// <param name="id">Id do registro / evento.</param>
        /// <response code="204">registro / evento removido com sucesso.</response>
        /// <response code="400">Não foi possível remover o registro / evento.</response>
        /// <response code="404">registro / evento não encontrado.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReiniciarRegistroEvento(string id)
        {
            var output = await _reiniciarRegistroEventoUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}