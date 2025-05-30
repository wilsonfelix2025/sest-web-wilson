using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.ObterCálculo;

namespace SestWeb.Api.UseCases.Cálculos.ObterCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(ObterCálculoOutput output)
        {
            switch (output.Status)
            {
                case ObterCálculoStatus.CálculoObtido:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case ObterCálculoStatus.CálculoNãoObtido:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
