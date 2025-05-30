using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.Recalcular;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoGradientes.Recalcular
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(RecalcularCálculoGradientesOutput output)
        {
            switch (output.Status)
            {
                case RecalcularCálculoGradientesStatus.CálculoRecalculado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case RecalcularCálculoGradientesStatus.CálculoNãoRecalculado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
