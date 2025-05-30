
using Microsoft.AspNetCore.Mvc;
using System.Net;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase.LerArquivoPoçoWebUseCase;

namespace SestWeb.Api.UseCases.Importação.ImportarPoçoWeb.LerArquivoPoçoWeb
{
    public class LerArquivoPoçoWebPresenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(LerArquivoPoçoWebOutput output)
        {
            switch (output.Status)
            {
                case LerArquivoPoçoWebStatus.LeituraComSucesso:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Retorno });
                    break;
                case LerArquivoPoçoWebStatus.LeituraSemSucesso:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
