using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro;

namespace SestWeb.Api.UseCases.Filtros.EditarFiltro
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(EditarFiltroOutput output)
        {
            switch (output.Status)
            {
                case EditarFiltroStatus.FiltroEditado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Perfil, output.Cálculo });
                    break;
                case EditarFiltroStatus.FiltroNãoEditado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
