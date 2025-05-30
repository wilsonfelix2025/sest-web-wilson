using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.TrocarSenha;

namespace SestWeb.Api.UseCases.Usuário.TrocarSenha
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuarioController : Controller
    {

        private readonly ITrocarSenhaUseCase _useCase;
        private readonly Presenter _presenter;

        public UsuarioController(ITrocarSenhaUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }
        
        /// <summary>
        /// Altera a senha do usuário autenticado.
        /// </summary>
        /// <param name="request">Senha antiga e nova senha.</param>
        /// <response code="200">Senha alterada com sucesso.</response>
        /// <response code="400">Senha não alterada.</response>
        /// <response code="401">Usuario não autorizado.</response>
        [HttpPost("trocar-senha")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> TrocarSenha([FromBody] TrocarSenhaRequest request)
        {
            var idUsuario = User.Identity.Name;
            var output = await _useCase.Execute(idUsuario, request.SenhaAntiga, request.NovaSenha);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
