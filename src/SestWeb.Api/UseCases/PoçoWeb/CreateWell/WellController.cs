using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.CreateWell;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.PoçoWeb.CreateWell
{
    ///<inheritdoc />
    [Route("api/pocoweb/wells")]
    [ApiController]
    public class WellController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly ICreateWellUseCase _useCase;

        ///<inheritdoc />
        public WellController(Presenter presenter, ICreateWellUseCase useCase)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Cria um Poços.
        /// </summary>
        /// <param name="request">Objeto request contendo nome do poço e id do campo</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWell(WellRequest request)
        {
            CreateWellOutput output = await _useCase.Execute(Request.Headers["Authorization"].ToString(), request.Name, request.OilFieldId);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
