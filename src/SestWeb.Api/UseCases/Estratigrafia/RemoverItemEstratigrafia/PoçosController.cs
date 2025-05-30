using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.RemoverItemEstratigrafia;

namespace SestWeb.Api.UseCases.Estratigrafia.RemoverItemEstratigrafia
{
    [Route("api/pocos/{id}/remover-item-estratigrafia")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IRemoverItemEstratigrafiaUseCase _removerItemEstratigrafiaUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IRemoverItemEstratigrafiaUseCase removerItemEstratigrafiaUseCase, Presenter presenter)
        {
            _removerItemEstratigrafiaUseCase = removerItemEstratigrafiaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove um item de estratigrafia.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Dados necessários para remover um item de estratigrafia.</param>
        /// <response code="200">Item de estratigrafia do poço foi adicionado com sucesso.</response>
        /// <response code="400">Não foi possível adicionar item de estratigrafia.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverItemEstratigrafia([FromRoute]string id, [FromBody]RemoverItemEstratigrafiaRequest request)
        {
            var output = await _removerItemEstratigrafiaUseCase.Execute(id, request.Tipo, request.Pm);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}