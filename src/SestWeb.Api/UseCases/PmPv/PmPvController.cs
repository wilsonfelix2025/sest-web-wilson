using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PmPvUseCases;

namespace SestWeb.Api.UseCases.PmPv
{
    [Route("api/{proftype}/convert-pmpv")]
    [ApiController]
    public class PmPvController : ControllerBase
    {
        private readonly IPmPvUseCase _pmPvUseCase;
        private readonly Presenter _presenter;

        public PmPvController(IPmPvUseCase pmPvUseCase, Presenter presenter)
        {
            _pmPvUseCase = pmPvUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Converte todos os elementos de pm para pv ou de pv para pm.
        /// </summary>
        /// <param name="proftype">pm ou pv.</param>
        /// <response code="200">Conversão Pm/Pv realizada.</response>
        /// <response code="400">Não foi possível realizar a conversão Pm/Pv.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConverterPmPv([FromRoute] string proftype)
        {
            var output = await _pmPvUseCase.Execute(proftype);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
