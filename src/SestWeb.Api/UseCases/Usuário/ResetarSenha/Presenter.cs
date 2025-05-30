using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.ResetarSenha;

namespace SestWeb.Api.UseCases.Usuário.ResetarSenha
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }
        public void Populate(ResetarSenhaUseCaseOutput output)
        {
            switch (output.Status)
            {
                case ResetarSenhaUseCaseStatus.SenhaResetada:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ResetarSenhaUseCaseStatus.SenhaNãoResetada:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
