using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0Acompanhamento;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularK0Acompanhamento
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CalcularK0AcompanhamentoOutput output)
        {
            switch (output.Status)
            {
                case CalcularK0AcompanhamentoStatus.CálculoK0AcompanhamentoEfetuado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Perfil });
                    break;
                case CalcularK0AcompanhamentoStatus.CálculoK0AcompanhamentoNãoEfetuado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
