using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho;

namespace SestWeb.Api.UseCases.Perfil.ObterPerfisParaTrecho
{
    [Route("api/pocos/{id}/obter-perfis-trecho")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterPerfisTrechoUseCase _useCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterPerfisTrechoUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém os perfis de um poço para ser utilizado na funcionalidade preencher trecho.
        /// </summary>
        /// <returns>Coleção contendo os perfis do poço indicado que podem ser usados na funcionalidade.</returns>
        /// <response code="200">Retorna a coleção contendo os perfis. A coleção pode ser vazia.</response>
        /// <response code="400">Não foi possível obter a coleção.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPerfisParaTrecho([FromRoute] string id)
        {
            var output = await _useCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
