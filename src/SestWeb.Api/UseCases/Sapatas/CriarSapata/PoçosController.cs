using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.SapataUseCases.CriarSapata;

namespace SestWeb.Api.UseCases.Sapatas.CriarSapata
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/criar-sapata")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly ICriarSapataUseCase _criarSapataUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(ICriarSapataUseCase criarSapataUseCase, Presenter presenter)
        {
            _criarSapataUseCase = criarSapataUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria uma nova sapata.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Informações necessárias para a criação de sapata.</param>
        /// <response code="201">A nova sapata foi criada.</response>
        /// <response code="400">A sapata não foi criada.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarSapata([FromRoute] string id, [FromBody] CriarSapataRequest request)
        {
            var output = await _criarSapataUseCase.Execute(id, request.ProfundidadeMedida, request.DiâmetroSapata);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}