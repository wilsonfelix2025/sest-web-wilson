using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.CriarCálculo;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoGradientes.CriarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CriarCálculoGradientesOutput output)
        {
            switch (output.Status)
            {
                case CriarCálculoGradientesStatus.CálculoCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case CriarCálculoGradientesStatus.CálculoNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
