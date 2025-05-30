using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ExportarRegistros;
using System.Net;

namespace SestWeb.Api.UseCases.ExportarRegistros
{
    public class ExportarRegistrosPresenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(ExportarRegistrosOutput output)
        {
            switch (output.Status)
            {
                case ExportarRegistrosStatus.ExportaçãoBemSucedida:
                    ViewModel = new OkObjectResult(new { output.NomeArquivo });
                    break;
                case ExportarRegistrosStatus.ExportaçãoNãoConcluída:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
