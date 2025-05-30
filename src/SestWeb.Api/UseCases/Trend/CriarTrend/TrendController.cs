using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.TrendUseCases.CriarTrend;

namespace SestWeb.Api.UseCases.Trend.CriarTrend
{
    [Route("api/criar-trend")]
    [ApiController]
    public class TrendController : ControllerBase
    {
        private readonly ICriarTrendUseCase _useCase;
        private readonly Presenter _presenter;

        public TrendController(ICriarTrendUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria um novo trend.
        /// </summary>
        /// <param name="idPerfil">Id do perfil que terá o trend criado.</param>
        /// <returns>Um novo trend criado.</returns>
        /// <response code="201">Retorna o novo trend criado.</response>
        /// <response code="400">O trend não foi criado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarTrend(string idPerfil)
        {
            var output = await _useCase.Execute(idPerfil);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
