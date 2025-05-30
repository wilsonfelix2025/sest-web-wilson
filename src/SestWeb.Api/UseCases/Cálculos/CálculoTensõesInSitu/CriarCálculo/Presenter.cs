using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarCálculo;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CriarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; } 

        internal void Populate(CriarCálculoTensõesInSituOutput output)
        {
            switch (output.Status)
            {
                case CriarCálculoTensõesInSituStatus.CálculoCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case CriarCálculoTensõesInSituStatus.CálculoNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
