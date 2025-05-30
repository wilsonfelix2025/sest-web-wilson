using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.ResetarSenha;

namespace SestWeb.Api.UseCases.Usuário.ResetarSenha
{
    [Route("api/usuarios")]
    public class UsuarioController : Controller
    {
        private readonly IResetarSenhaUseCase _useCase;
        private readonly Presenter _presenter;

        public UsuarioController(IResetarSenhaUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Reseta senha do usuário.
        /// </summary>
        /// <param name="request">Email do usuario, nova senha e codigo de reset.</param>
        /// <response code="200">Senha resetada com sucesso.</response>
        /// <response code="400">Senha não resetada.</response>
        [HttpPost("resetar-senha")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetarSenha([FromBody] ResetarSenhaRequest request)
        {
            var output = await _useCase.Execute(request.Email, request.Senha, request.Codigo);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
