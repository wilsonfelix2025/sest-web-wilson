using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UsuárioUseCases.EsqueceuSenha;

namespace SestWeb.Api.UseCases.Usuário.EsqueceuSenha
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }
        public void Populate(EsqueceuSenhaUseCaseOutput output)
        {
            switch (output.Status)
            {
                case EsqueceuSenhaUseCaseStatus.EmailEnviado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, codigoReset = output.CodigoReset });
                    break;
                case EsqueceuSenhaUseCaseStatus.EmailNaoEnviado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
