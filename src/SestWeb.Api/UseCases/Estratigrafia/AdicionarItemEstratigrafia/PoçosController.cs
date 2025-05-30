using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.AdicionarItemEstratigrafia;

namespace SestWeb.Api.UseCases.Estratigrafia.AdicionarItemEstratigrafia
{
    [Route("api/pocos/{id}/adicionar-item-estratigrafia")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IAdicionarItemEstratigrafiaUseCase _adicionarItemEstratigrafiaUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IAdicionarItemEstratigrafiaUseCase adicionarItemEstratigrafiaUseCase, Presenter presenter)
        {
            _adicionarItemEstratigrafiaUseCase = adicionarItemEstratigrafiaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Adiciona um item de estratigrafia.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Dados necessários para adicionar um item de estratigrafia.</param>
        /// <response code="200">Item de estratigrafia do poço foi adicionado com sucesso.</response>
        /// <response code="400">Não foi possível adicionar item de estratigrafia.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdicionarItemEstratigrafia([FromRoute]string id, [FromBody] AdicionarItemEstratigrafiaRequest request)
        {
            var input = new AdicionarItemEstratigrafiaInput(id, request.Tipo, request.PM, request.Sigla, request.Descrição, request.Idade);
            var output = await _adicionarItemEstratigrafiaUseCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}