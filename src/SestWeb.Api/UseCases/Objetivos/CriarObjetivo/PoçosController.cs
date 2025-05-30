using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ObjetivoUseCases.CriarObjetivo;

namespace SestWeb.Api.UseCases.Objetivos.CriarObjetivo
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/criar-objetivo")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly ICriarObjetivoUseCase _criarObjetivoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(ICriarObjetivoUseCase criarObjetivoUseCase, Presenter presenter)
        {
            _criarObjetivoUseCase = criarObjetivoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria um novo objetivo.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Informações necessárias para a criação do objetivo.</param>
        /// <response code="201">O novo objetivo foi criado.</response>
        /// <response code="400">O objetivo não foi criado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarObjetivo([FromRoute] string id, [FromBody] CriarObjetivoRequest request)
        {
            var output = await _criarObjetivoUseCase.Execute(id, request.ProfundidadeMedida, request.TipoObjetivo);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}