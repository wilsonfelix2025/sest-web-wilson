using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrendUseCases.ObterTrend;

namespace SestWeb.Api.UseCases.Trend.ObterTrend
{
    [Route("api/obter-trend")]
    [ApiController]
    public class TrendController : ControllerBase
    {
        private readonly IObterTrendUseCase _useCase;
        private readonly Presenter _presenter;

        public TrendController(IObterTrendUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtem o trend do perfil
        /// </summary>
        /// <param name="idPerfil">Id do perfil que se deseja obter o trend</param>
        /// <returns>Trend do perfil.</returns>
        /// <response code="200">Retorna o trend.</response>
        /// <response code="400">O trend não foi obtido.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterTrend(string idPerfil)
        {
            var output = await _useCase.Execute(idPerfil);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
