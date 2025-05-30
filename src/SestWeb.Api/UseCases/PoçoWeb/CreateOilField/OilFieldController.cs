using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.CreateOilField;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.CreateOilField
{
    /// <inheritdoc />
    [Route("api/pocoweb/oilfields")]
    [ApiController]
    public class OilFieldController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly ICreateOilFieldUseCase _useCase;

        ///<inheritdoc />
        public OilFieldController(ICreateOilFieldUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Cria um campo.
        /// </summary>
        /// <param name="request">Objeto request contendo nome do campo, sua url e id a unidade operacional.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOilField(OilFieldRequest request)
        {
            if (request.Url == null) {
                // criar no poco web e pegar os dados
            }
            var output = await _useCase.Execute(request.Name, request.Url, request.opUnitId);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
