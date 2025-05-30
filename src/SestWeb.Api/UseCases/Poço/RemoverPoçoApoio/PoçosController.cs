using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.RemoverPoçoApoio;

namespace SestWeb.Api.UseCases.Poço.RemoverPoçoApoio
{
    /// <inheritdoc />
    [Route("api/pocos/{id}/remove-apoio")]
    [ApiController]
    public class PoçosController : ControllerBase
    {
        private readonly IRemoverPoçoApoioUseCase _useCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PoçosController(IRemoverPoçoApoioUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarPoço([FromRoute]string id, string idPoçoApoio)
        {
            var output = await _useCase.Execute(id, idPoçoApoio);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}