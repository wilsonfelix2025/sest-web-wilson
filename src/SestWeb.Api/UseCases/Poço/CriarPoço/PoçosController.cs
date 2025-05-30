using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.CriarPoço;

namespace SestWeb.Api.UseCases.Poço.CriarPoço
{
    /// <inheritdoc />
    [Route("api/pocos")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly ICriarPoçoUseCase _criarPoçoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(ICriarPoçoUseCase criarPoçoUseCase, Presenter presenter)
        {
            _criarPoçoUseCase = criarPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria um novo poço.
        /// </summary>
        /// <param name="criarPoçoRequest">Objeto contendo nome e tipo do poço.</param>
        /// <returns>Um novo poço criado.</returns>
        /// <response code="201">Retorna o novo poço criado.</response>
        /// <response code="400">O poço não foi criado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarPoço(CriarPoçoRequest criarPoçoRequest)
        {
            var output = await _criarPoçoUseCase.Execute(criarPoçoRequest.Nome, criarPoçoRequest.TipoPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}