using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.ConfirmarEmail;

namespace SestWeb.Api.UseCases.Usuário.ConfirmarEmail
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }
        public void Populate(ConfirmarEmailOutput output)
        {
            switch (output.Status)
            {
                case ConfirmarEmailStatus.EmailConfirmado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ConfirmarEmailStatus.EmailNaoConfirmado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
