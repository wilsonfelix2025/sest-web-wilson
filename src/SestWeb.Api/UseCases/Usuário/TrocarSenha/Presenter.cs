using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.TrocarSenha;

namespace SestWeb.Api.UseCases.Usuário.TrocarSenha
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }
        public void Populate(TrocarSenhaUseCaseOutput output)
        {
            switch (output.Status)
            {
                case TrocarSenhaUseCaseStatus.SenhaTrocada:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case TrocarSenhaUseCaseStatus.SenhaNãoTrocada:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
