using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetWell;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.GetWell
{
    [Route("api/pocoweb/well")]
    [ApiController]
    public class WellController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IGetWellUseCase _useCase;

        public WellController(IGetWellUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Obtem um poço pelo id.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetWell")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWell(string id)
        {
            var output = await _useCase.Execute(id);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
