using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase;

namespace SestWeb.Api.UseCases.Importação.ImportarArquivo
{
    public class ImportarArquivoPresenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(ImportarArquivoOutput output)
        {
            switch (output.Status)
            {
                case ImportarArquivoStatus.ImportadoComSucesso:
                    ViewModel = new OkObjectResult(new ImportarArquivoModel(output.Mensagem));
                    break;
                case ImportarArquivoStatus.ImportaçãoComFalha:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                case ImportarArquivoStatus.ImportaçãoComFalhasDeValidação:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
