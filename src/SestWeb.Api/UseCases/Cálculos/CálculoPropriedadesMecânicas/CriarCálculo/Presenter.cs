using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarCálculo;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CriarCálculoPropMecOutput output)
        {
            switch (output.Status)
            {
                case CriarCálculoPropMecStatus.CálculoPropMecCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case CriarCálculoPropMecStatus.CálculoPropMecNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
