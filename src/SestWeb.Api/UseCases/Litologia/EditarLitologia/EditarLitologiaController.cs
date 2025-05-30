using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.EditarLitologia;

namespace SestWeb.Api.UseCases.Litologia.EditarLitologia
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/editar-litologia")]
    [ApiController]
    public class EditarLitologiaController : ControllerBase
    {
        private readonly IEditarLitologiaUseCase _editarLitologiaUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public EditarLitologiaController(IEditarLitologiaUseCase editarLitologiaUseCase, Presenter presenter)
        {
            _editarLitologiaUseCase = editarLitologiaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Edita uma litologia do poço.
        /// </summary>
        /// <param name="id">Id do poço a editar.</param>
        /// <param name="request">Informações necessárias para a edição da litologia.</param>
        /// <response code="201">A nova litologia foi criada.</response>
        /// <response code="400">A litologia não foi criada.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarLitologia([FromRoute] string id, EditarLitologiaRequest request)
        {
            var output = await _editarLitologiaUseCase.Execute(id, request.TipoLitologia, request.Pontos);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}