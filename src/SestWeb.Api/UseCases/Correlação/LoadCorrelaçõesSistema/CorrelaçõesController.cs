using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.LoadCorrelaçõesSistema;

namespace SestWeb.Api.UseCases.Correlação.LoadCorrelaçõesSistema
{
    [Route("api/correlacoes/load-system-corrs")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly ILoadCorrelaçõesSistemaUseCase _loadCorrelaçõesSistemaUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(ILoadCorrelaçõesSistemaUseCase loadCorrelaçõesSistemaUseCase, Presenter presenter)
        {
            _loadCorrelaçõesSistemaUseCase = loadCorrelaçõesSistemaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Carrega ou atualiza as correlações do sistema.
        /// </summary>
        /// <response code="200">Retorna todas correlações carregadas.</response>
        /// <response code="400">Não foi possível carregar as correlações.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoadSystemCorrs()
        {
            var output = await _loadCorrelaçõesSistemaUseCase.Execute();
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
