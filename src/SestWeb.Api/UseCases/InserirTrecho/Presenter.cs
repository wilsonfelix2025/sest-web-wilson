using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.InserirTrechoUseCase;
using System.Net;

namespace SestWeb.Api.UseCases.InserirTrecho
{
    public class Presenter
    {
        public IActionResult ViewModel { get; set; }

        internal void Populate(InserirTrechoOutput output)
        {
            switch (output.Status)
            {
                case InserirTrechoStatus.InserirTrechoComSucesso:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Perfil } );
                    break;
                case InserirTrechoStatus.InserirTrechoComFalha:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                case InserirTrechoStatus.InserirTrechoComFalhaDeValidação:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
            
        }
    }
}
