using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoPP;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularNormalizaçãoPP
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CalcularNormalizaçãoPPOutput output)
        {
            switch (output.Status)
            {
                case CalcularNormalizaçãoPPStatus.CálculoNormalizaçãoPPEfetuado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Retorno });
                    break;
                case CalcularNormalizaçãoPPStatus.CálculoNormalizaçãoPPNãoEfetuado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }

    }
}
