using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfil;

namespace SestWeb.Api.UseCases.Perfil.ObterPerfil
{
    [Route("api/perfis")]
    [ApiController]
    public class PerfisController : ControllerBase
    {
        private readonly IObterPerfilUseCase _obterPerfilUseCase;
        private readonly Presenter _presenter;

        public PerfisController(IObterPerfilUseCase obterPerfilUseCase, Presenter presenter)
        {
            _obterPerfilUseCase = obterPerfilUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém um perfil pelo id.
        /// </summary>
        /// <param name="id">Id do perfil.</param>
        /// <response code="200">Retorna o perfil desejado.</response>
        /// <response code="400">Não foi possível obter o perfil desejado.</response>
        /// <response code="404">Não foi encontrado perfil com o id informado.</response>
        [HttpGet("{id}", Name = "ObterPerfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPerfil(string id)
        {
            var output = await _obterPerfilUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}