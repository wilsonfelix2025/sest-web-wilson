using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoLDA;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularNormalizaçãoLDA
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CalcularNormalizaçãoLDAOutput output)
        {
            switch (output.Status)
            {
                case CalcularNormalizaçãoLDAStatus.CálculoNormalizaçãoLDAEfetuado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Retorno });
                    break;
                case CalcularNormalizaçãoLDAStatus.CálculoNormalizaçãoLDANãoEfetuado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }

    }
}
