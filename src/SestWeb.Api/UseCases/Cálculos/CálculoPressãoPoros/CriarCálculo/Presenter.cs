using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.CriarCálculo;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPressãoPoros.CriarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CriarCálculoPressãoPorosOutput output)
        {
            switch (output.Status)
            {
                case CriarCálculoPressãoPorosStatus.CálculoPressãoPorosCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case CriarCálculoPressãoPorosStatus.CálculoPressãoPorosNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
