using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetOpUnit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.GetOpUnit
{
    [Route("api/pocoweb/opunit")]
    [ApiController]
    public class OpUnitController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IGetOpUnitUseCase _useCase;

        public OpUnitController(IGetOpUnitUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Obtem uma unidade operacional pelo id.
        /// </summary>
        /// <param name="id">Id da unidade operacional.</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetOpUnit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOpUnit(string id)
        {
            var output = await _useCase.Execute(id);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
