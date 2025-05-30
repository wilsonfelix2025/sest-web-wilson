using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.CreateOpUnit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.CreateOpUnit
{
    /// <inheritdoc />
    [Route("api/pocoweb/opunits")]
    [ApiController]
    public class OpUnitController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly ICreateOpUnitUseCase _useCase;

        /// <inheritdoc />
        public OpUnitController(ICreateOpUnitUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Cria uma unidade operacional.
        /// </summary>
        /// <param name="request">Objeto request contendo o nome da unidade operacional e sua url</param>
        /// <response code="201">Retorna o nome da unidade operacional criada.</response>
        /// <response code="400">Informa que a unidade operacional não foi criada.</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOpUnit(OpUnitRequest request)
        {
            var output = await _useCase.Execute(request.Name, request.Url);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
