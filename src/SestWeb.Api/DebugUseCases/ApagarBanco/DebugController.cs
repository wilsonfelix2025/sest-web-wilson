using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.DebugUseCases.ApagarBanco;

namespace SestWeb.Api.DebugUseCases.ApagarBanco
{
    /// <inheritdoc />
    [Route("api/debug/apagar-database")]
    public class DebugController : Controller
    {
        private readonly Presenter _presenter;
        private readonly IApagarBancoUseCase _useCase;

        /// <inheritdoc />
        public DebugController(IApagarBancoUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        ///     Deleta o banco de dados eliminando todas as suas informações.
        /// </summary>
        /// <returns>status</returns>
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                await _useCase.Execute();
                _presenter.Populate(true);
            }
            catch (Exception)
            {
                _presenter.Populate(false);
            }

            return _presenter.ViewModel;
        }
    }
}