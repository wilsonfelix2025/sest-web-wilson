
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.MontagemPerfis;
using System.Net;

namespace SestWeb.Api.UseCases.MontagemPerfis
{
    public class Presenter
    {
        public IActionResult ViewModel { get; set; }

        internal void Populate(MontarPerfisOutput output)
        {
            switch (output.Status)
            {
                case MontarPerfisStatus.MontarPerfisComSucesso:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, listaPerfis = output.ListaPerfis, litologia = output.Litologia });
                    break;
                case MontarPerfisStatus.MontarPerfisComFalha:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                case MontarPerfisStatus.MontarPerfisComFalhaDeValidação:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }

        }
    }
}
