using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetOilField;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.GetOilField
{
    [Route("api/pocoweb/oilfield")]
    [ApiController]
    public class OilFieldController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IGetOilFieldUseCase _useCase;

        public OilFieldController(IGetOilFieldUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Obtem um campo pelo id.
        /// </summary>
        /// <param name="id">Id do campo.</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetOilField")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOilField(string id)
        {
            var output = await _useCase.Execute(id);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
