using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisPorTipo;

namespace SestWeb.Api.UseCases.Perfil.ObterPerfisPorTipo
{
    [Route("api/perfis/obter-perfis-por-tipo")]
    [ApiController]
    public class PerfisController : ControllerBase
    {
        private readonly IObterPerfisPorTipoUseCase _obterPerfisPorTipoUseCase;
        private readonly Presenter _presenter;

        public PerfisController(IObterPerfisPorTipoUseCase obterPerfisPorTipoUseCase, Presenter presenter)
        {
            _obterPerfisPorTipoUseCase = obterPerfisPorTipoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém os perfis de um poço de um determinado tipo.
        /// </summary>
        /// <param name="request">Informações necessárias para a obtenção dos perfis.</param>
        /// <returns>Coleção contendo os perfis do poço indicado que possuem o mnemônico fornecido.</returns>
        /// <response code="200">Retorna a coleção contendo os perfis. A coleção pode ser vazia.</response>
        /// <response code="400">Não foi possível obter a coleção.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPerfisPorTipo(ObterPerfisPorTipoRequest request)
        {
            var output = await _obterPerfisPorTipoUseCase.Execute(request.IdPoço, request.Mnemônico);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}