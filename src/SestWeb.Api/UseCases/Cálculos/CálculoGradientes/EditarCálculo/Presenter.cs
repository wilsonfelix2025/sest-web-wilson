using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.EditarCálculo;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoGradientes.EditarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(EditarCálculoGradientesOutput output)
        {
            switch (output.Status)
            {
                case EditarCálculoGradientesStatus.CálculoEditado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo, output.PerfisAlterados });
                    break;
                case EditarCálculoGradientesStatus.CálculoNãoEditado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
