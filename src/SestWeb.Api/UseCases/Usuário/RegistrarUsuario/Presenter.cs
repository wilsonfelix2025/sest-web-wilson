using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.RegistrarUsuario;

namespace SestWeb.Api.UseCases.Usuário.RegistrarUsuario
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        public void Populate(RegistrarUsuarioOutput output)
        {
            switch(output.Status)
            {
                case RegistrarUsuarioStatus.UsuarioCriado:
                    ViewModel = new CreatedResult("", new { email = output.Email, mensagem = output.Mensagem, idUsuario = output.IdUsuario, codigoConfirmaçãoEmail = output.CodigoConfirmaçãoEmail });
                    break;
                case RegistrarUsuarioStatus.UsuarioNaoCriado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
