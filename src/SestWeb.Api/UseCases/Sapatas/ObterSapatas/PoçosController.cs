using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.SapataUseCases.ObterSapatas;

namespace SestWeb.Api.UseCases.Sapatas.ObterSapatas
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/obter-sapatas")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IObterSapatasUseCase _obterSapatasUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(IObterSapatasUseCase obterSapatasUseCase, Presenter presenter)
        {
            _obterSapatasUseCase = obterSapatasUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém as sapatas de um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns>Coleção com as sapatas do poço.</returns>
        /// <response code="200">Retorna coleção com as sapatas do poço.</response>
        /// <response code="400">Não foi possível obter as sapatas do poço.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpGet(Name = "ObterSapatas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterSapatas(string id)
        {
            var output = await _obterSapatasUseCase.Execute(id);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}