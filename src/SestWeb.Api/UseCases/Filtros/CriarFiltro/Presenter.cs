using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro;

namespace SestWeb.Api.UseCases.Filtros.CriarFiltro
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CriarFiltroOutput output)
        {
            switch (output.Status)
            {
                case CriarFiltroStatus.FiltroCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Perfil, output.Cálculo });
                    break;
                case CriarFiltroStatus.FiltroNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
