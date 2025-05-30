using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.EditarCálculo;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.EditarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(EditarCálculoPerfisOutput output)
        {
            switch (output.Status)
            {
                case EditarCálculoPerfisStatus.CálculoPerfisEditado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo, output.PerfisAlterados });
                    break;
                case EditarCálculoPerfisStatus.CálculoPerfisNãoEditado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
