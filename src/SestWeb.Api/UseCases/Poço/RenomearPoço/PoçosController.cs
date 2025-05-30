using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.RenomearPoço;

namespace SestWeb.Api.UseCases.Poço.RenomearPoço
{
    [Route("api/pocos/{id}/renomear-poco")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IRenomearPoçoUseCase _renomearPoçoUseCase;
        private readonly Presenter _presenter;

        public PoçosController(IRenomearPoçoUseCase renomearPoçoUseCase, Presenter presenter)
        {
            _renomearPoçoUseCase = renomearPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Renomea um poço.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Informações necessárias para renomear o poço.</param>
        /// <response code="200">O poço foi renomeado com sucesso.</response>
        /// <response code="400">Não foi possível renomear o poço.</response>
        /// <response code="404">O poço não foi encontrado.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RenomearPoço(string id, RenomearPoçoRequest request)
        {
            var output = await _renomearPoçoUseCase.Execute(id, request.NomePoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}