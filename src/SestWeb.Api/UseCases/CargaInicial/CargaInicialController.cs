using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.CargaInicial;

namespace SestWeb.Api.UseCases.CargaInicial
{
    [Route("api/carga-inicial")]
    [ApiController]
    public class CargaInicialController : ControllerBase
    {
        private readonly ICargaInicialUseCase _cargaInicialUseCase;
        private readonly Presenter _presenter;

        public CargaInicialController(ICargaInicialUseCase cargaInicialUseCase, Presenter presenter)
        {
            _cargaInicialUseCase = cargaInicialUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Realiza a carga inicial do banco.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FillDatabase()
        {
            try
            {
                await _cargaInicialUseCase.Execute(Request.Headers["Authorization"].ToString());
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
