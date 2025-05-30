using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    public class ImportarDadosPresenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(ImportarDadosOutput output)
        {
            switch (output.Status)
            {
                case ImportarDadosStatus.ImportadoComSucesso:
                    ViewModel = new OkObjectResult(new ImportarDadosModel(output.Mensagem, output.PerfisImportados));
                    break;
                case ImportarDadosStatus.ImportaçãoComFalha:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                case ImportarDadosStatus.ImportaçãoComFalhasDeValidação:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
