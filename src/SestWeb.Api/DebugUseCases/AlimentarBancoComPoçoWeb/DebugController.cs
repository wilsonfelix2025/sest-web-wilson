using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.DebugUseCases.AlimentarBancoComPocoWeb;

namespace SestWeb.Api.DebugUseCases.AlimentarBancoComPoçoWeb
{
    /// <inheritdoc />
    [Route("api/debug/alimentar-database")]
    public class DebugController : Controller
    {
        private readonly Presenter _presenter;
        private readonly IAlimentarBancoComPoçoWebUseCase _useCase;

        /// <inheritdoc />
        public DebugController(IAlimentarBancoComPoçoWebUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Limpa o banco e carrega o banco de dados com dados do poço web
        /// </summary>
        /// <returns>status</returns>
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                var result = await _useCase.Execute(Request.Headers["Authorization"].ToString());
                _presenter.Populate(true, result);
            }
            catch (Exception)
            {
                _presenter.Populate(false, null);
            }

            return _presenter.ViewModel;
        }
    }
}