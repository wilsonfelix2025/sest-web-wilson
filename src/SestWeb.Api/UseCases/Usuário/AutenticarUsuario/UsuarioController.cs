using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.AutenticarUsuario;

namespace SestWeb.Api.UseCases.Usuário.AutenticarUsuario
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IAutenticarUsuarioUseCase _useCase;

        public UsuarioController(IAutenticarUsuarioUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }
        
        /// <summary>
        /// Autenticar usuário.
        /// </summary>
        /// <param name="request">Informações necessárias para autenticação do usuário</param>
        /// <response code="200">Retorna email, username e token do usuário autenticado.</response>
        /// <response code="400">Informa mensagens de erro de autenticação.</response>
        [HttpPost("autenticar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AutenticarUsuario(AutenticarUsuarioRequest request)
        {
            var output = await _useCase.Execute(request.Email, request.Senha);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
