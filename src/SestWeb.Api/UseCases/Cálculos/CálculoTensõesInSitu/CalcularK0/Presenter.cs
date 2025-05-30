using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularK0
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CalcularK0Output output)
        {
            switch (output.Status)
            {
                case CalcularK0Status.CálculoK0Efetuado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Retorno });
                    break;
                case CalcularK0Status.CálculoK0NãoEfetuado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
