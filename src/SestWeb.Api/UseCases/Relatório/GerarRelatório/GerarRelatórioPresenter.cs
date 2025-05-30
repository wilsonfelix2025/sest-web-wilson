using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.GerarRelatórioUseCase;

namespace SestWeb.Api.UseCases.GerarRelatório
{
    public class GerarRelatórioPresenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(GerarRelatórioOutput output)
        {
            switch (output.Status)
            {
                case GerarRelatórioStatus.Sucesso:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Caminho });
                    break;
                case GerarRelatórioStatus.Falha:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
