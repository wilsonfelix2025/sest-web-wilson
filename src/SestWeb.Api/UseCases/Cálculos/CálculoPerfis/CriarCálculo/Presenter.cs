using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.CriarCálculo;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.CriarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CriarCálculoPerfisOutput output)
        {
            switch (output.Status)
            {
                case CriarCálculoPerfisStatus.CálculoPerfisCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case CriarCálculoPerfisStatus.CálculoPerfisNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
