using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.EditarCálculo;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.EditarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; } 

        internal void Populate(EditarCálculoTensõesInSituOutput output)
        {
            switch (output.Status)
            {
                case EditarCálculoTensõesInSituStatus.CálculoEditado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo, output.PerfisAlterados });
                    break;
                case EditarCálculoTensõesInSituStatus.CálculoNãoEditado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
