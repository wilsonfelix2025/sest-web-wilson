using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.CriarPerfil;

namespace SestWeb.Api.UseCases.Perfil.CriarPerfil
{
    /// <inheritdoc />
    [Route("api/perfis")]
    [ApiController]
    public class PerfisController : ControllerBase
    {
        private readonly ICriarPerfilUseCase _criarPerfilUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public PerfisController(ICriarPerfilUseCase criarPerfilUseCase, Presenter presenter)
        {
            _criarPerfilUseCase = criarPerfilUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria um novo perfil.
        /// </summary>
        /// <param name="criarPerfilRequest">Informações necessárias para a criação do perfil.</param>
        /// <returns>Um novo perfil criado.</returns>
        /// <response code="201">Retorna o novo perfil criado.</response>
        /// <response code="400">O perfil não foi criado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarPerfil(CriarPerfilRequest criarPerfilRequest)
        {
            var output = await _criarPerfilUseCase.Execute(criarPerfilRequest.IdPoço, criarPerfilRequest.Nome, criarPerfilRequest.Mnemônico);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}