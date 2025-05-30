using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrendUseCases.RemoverTrend;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Trend.RemoverTrend
{
    [Route("api/remover-trend")]
    [ApiController]
    public class TrendController : ControllerBase
    {
        private readonly IRemoverTrendUseCase _useCase;
        private readonly Presenter _presenter;

        public TrendController(IRemoverTrendUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove o trend do perfil
        /// </summary>
        /// <param name="idPerfil">Id do perfil que terá o trend removido.</param>
        /// <returns>Trend excluido.</returns>
        /// <response code="204">Trend removido.</response>
        /// <response code="400">O trend não foi removido.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverTrend(string idPerfil)
        {
            var output = await _useCase.Execute(idPerfil);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
