using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.ConfirmarEmail;

namespace SestWeb.Api.UseCases.Usuário.ConfirmarEmail
{
    [Route("api/usuarios")]
    public class UsuarioController : Controller
    {
        private readonly IConfirmarEmailUseCase _useCase;
        private readonly Presenter _presenter;

        public UsuarioController(IConfirmarEmailUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Confirmar email do usuario.
        /// </summary>
        /// <param name="request">Informações necessárias para confirmação do email do usuário.</param>
        /// <response code="200">Indica email confirmado com sucesso. Retorna mensagem.</response>
        /// <response code="400">Informa erro na confirmação de email.</response>
        [HttpPost("confirmarEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmarEmail([FromBody]ConfirmarEmailRequest request)
        {
            var output = await _useCase.Execute(request.IdUsuario, request.Codigo);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
