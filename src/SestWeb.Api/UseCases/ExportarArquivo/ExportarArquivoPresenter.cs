using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ExportarArquivoUseCase;

namespace SestWeb.Api.UseCases.ExportarArquivo
{
    public class ExportarArquivoPresenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(ExportarArquivoOutput output)
        {
            switch (output.Status)
            {
                case ExportarArquivoStatus.ExportaçãoBemSucedida:
                    ViewModel = new OkObjectResult(new { output.Arquivo });
                    break;
                case ExportarArquivoStatus.ExportaçãoNãoConcluída:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
