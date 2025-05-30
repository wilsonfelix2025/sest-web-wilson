using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.EditarCálculo;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.EditarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(EditarCálculoPropMecOutput output)
        {
            switch (output.Status)
            {
                case EditarCálculoPropMecStatus.CálculoPropMecEditado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo, output.PerfisAlterados });
                    break;
                case EditarCálculoPropMecStatus.CálculoPropMecNãoEditado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
