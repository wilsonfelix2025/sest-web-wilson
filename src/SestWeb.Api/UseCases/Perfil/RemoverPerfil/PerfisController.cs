using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.RemoverPerfil;

namespace SestWeb.Api.UseCases.Perfil.RemoverPerfil
{
    /// <inheritdoc />
    [Route("api/perfis")]
    [ApiController]
    public class PerfisController : ControllerBase
    {
        private readonly IRemoverPerfilUseCase _removerPerfilUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PerfisController(IRemoverPerfilUseCase removerPerfilUseCase, Presenter presenter)
        {
            _removerPerfilUseCase = removerPerfilUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove um perfil.
        /// </summary>
        /// <param name="idPerfil">Id do perfil.</param>
        /// <param name="idPoço">Id do poço.</param>
        /// <response code="204">Perfil removido com sucesso.</response>
        /// <response code="400">Não foi possível remover o perfil.</response>
        /// <response code="404">Perfil não encontrado.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverPerfil(string idPerfil, string idPoço)
        {
            var output = await _removerPerfilUseCase.Execute(idPerfil, idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}