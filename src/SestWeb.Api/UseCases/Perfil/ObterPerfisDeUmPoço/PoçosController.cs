using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisDeUmPoço;

namespace SestWeb.Api.UseCases.Perfil.ObterPerfisDeUmPoço
{
    [Route("api/pocos/{id}/obter-perfis")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterPerfisDeUmPoçoUseCase _obterPerfisDeUmPoçoUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterPerfisDeUmPoçoUseCase obterPerfisDeUmPoçoUseCase, Presenter presenter)
        {
            _obterPerfisDeUmPoçoUseCase = obterPerfisDeUmPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém os perfis de um poço.
        /// </summary>
        /// <returns>Coleção contendo os perfis do poço indicado.</returns>
        /// <response code="200">Retorna a coleção contendo os perfis. A coleção pode ser vazia.</response>
        /// <response code="400">Não foi possível obter a coleção.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPerfisDeUmPoço([FromRoute] string id)
        {
            var output = await _obterPerfisDeUmPoçoUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}