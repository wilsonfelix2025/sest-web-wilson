using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.CriarLitologia;

namespace SestWeb.Api.UseCases.Litologia.CriarLitologia
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/criar-litologia")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly ICriarLitologiaUseCase _criarLitologiaUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(ICriarLitologiaUseCase criarLitologiaUseCase, Presenter presenter)
        {
            _criarLitologiaUseCase = criarLitologiaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria uma nova litologia.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Informações necessárias para a criação da litologia.</param>
        /// <response code="201">A nova litologia foi criada.</response>
        /// <response code="400">A litologia não foi criada.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarLitologia([FromRoute] string id, CriarLitologiaRequest request)
        {
            var output = await _criarLitologiaUseCase.Execute(id, request.TipoLitologia);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}