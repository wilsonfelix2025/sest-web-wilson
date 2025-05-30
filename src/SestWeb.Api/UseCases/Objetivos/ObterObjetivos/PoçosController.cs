using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ObjetivoUseCases.ObterObjetivos;

namespace SestWeb.Api.UseCases.Objetivos.ObterObjetivos
{
    [Route("api/pocos/{id}/obter-objetivos")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterObjetivosUseCase _obterObjetivosUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IObterObjetivosUseCase obterObjetivosUseCase, Presenter presenter)
        {
            _obterObjetivosUseCase = obterObjetivosUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém os objetivos de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Coleção com os objetivos do poço.</returns>
        /// <response code="200">Retorna coleção com os objetivos do poço.</response>
        /// <response code="400">Não foi possível obter os objetivos do poço.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpGet(Name = "ObterObjetivos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterObjetivos(string id)
        {
            var output = await _obterObjetivosUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}