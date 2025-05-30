using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterTiposDePerfil;

namespace SestWeb.Api.UseCases.Perfil.ObterTiposDePerfil
{
    [Route("api/perfis/obter-tipos-de-perfil")]
    [ApiController]
    public class PerfisController : ControllerBase
    {
        private readonly IObterTiposDePerfilUseCase _obterTiposPerfilUseCase;
        private readonly Presenter _presenter;

        public PerfisController(IObterTiposDePerfilUseCase obterTiposPerfilUseCase, Presenter presenter)
        {
            _obterTiposPerfilUseCase = obterTiposPerfilUseCase;
            _presenter = presenter;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ObterTiposDePerfil()
        {
            var output = _obterTiposPerfilUseCase.Execute();
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
