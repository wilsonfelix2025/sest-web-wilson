using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.EsqueceuSenha;

namespace SestWeb.Api.UseCases.Usuário.EsqueceuSenha
{
    [Route("api/usuarios")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuarioController : Controller
    {
        private readonly IEsqueceuSenhaUseCase _useCase;
        private readonly Presenter _presenter;

        public UsuarioController(IEsqueceuSenhaUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Envia email de recuperação de senha.
        /// </summary>
        /// <param name="request">Email de destino.</param>
        /// <response code="200">Email enviado com sucesso.</response>
        /// <response code="400">Email não é de um usuário cadastrado.</response>
        [HttpPost("esqueceu-senha")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EnviarEmailEsqueceuSenha([FromBody]EsqueceuSenhaRequest request)
        {
            var output = await _useCase.Execute(request.Email);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
