using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.RegistrarUsuario;

namespace SestWeb.Api.UseCases.Usuário.RegistrarUsuario
{
    /// <inheritdoc />
    [Route("api/usuarios")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IRegistrarUsuarioUseCase _useCase;
        /// <inheritdoc />
        public UsuarioController(IRegistrarUsuarioUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Registra um usuário sem privilégios de administrador.
        /// </summary>
        /// <param name="request">Informações necessárias ao registro de usuário</param>
        /// <response code="201">Retorna email do usuário registrado.</response>
        /// <response code="400">Informa que o usuario não foi registrado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarUsuario(RegistrarUsuarioRequest request)
        {
            var output = await _useCase.Execute(request.Nome, request.Sobrenome, request.Email, request.Senha);

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
