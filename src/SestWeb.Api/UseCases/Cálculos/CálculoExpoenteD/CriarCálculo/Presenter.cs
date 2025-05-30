using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.CriarCálculo;

namespace SestWeb.Api.UseCases.Cálculos.CálculoExpoenteD.CriarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CriarCálculoExpoenteDOutput output)
        {
            switch (output.Status)
            {
                case CriarCálculoExpoenteDStatus.CálculoExpoenteDCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case CriarCálculoExpoenteDStatus.CálculoExpoenteDNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
