using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.BaixarRelatórioUseCase;

namespace SestWeb.Api.UseCases.BaixarRelatório
{
    public class BaixarRelatórioPresenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(BaixarRelatórioOutput output)
        {
            switch (output.Status)
            {
                case BaixarRelatórioStatus.Sucesso:
                    ViewModel = new OkObjectResult(new { output.Arquivo });
                    break;
                case BaixarRelatórioStatus.Falha:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
