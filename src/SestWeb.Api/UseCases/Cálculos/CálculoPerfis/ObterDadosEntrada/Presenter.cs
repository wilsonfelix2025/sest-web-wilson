using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.ObterDadosEntrada;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.ObterDadosEntrada
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(ObterDadosEntradaCálculoPerfisOutput output)
        {
            switch (output.Status)
            {
                case ObterDadosEntradaCálculoPerfisStatus.DadosObtidos:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo });
                    break;
                case ObterDadosEntradaCálculoPerfisStatus.DadosNãoObtidos:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
