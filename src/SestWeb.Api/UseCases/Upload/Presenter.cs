using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.UploadUseCase;

namespace SestWeb.Api.UseCases.Upload
{
    /// Presenter do caso de uso Upload arquivo
    public class Presenter
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        /// Popula a ViewModel
        /// </summary>
        /// <param name="output">Resultado do caso de uso</param>
        internal void Populate(UploadArquivoOutput output)
        {
            switch (output.Status)
            {
                case UploadArquivoStatus.UploadRealizado:
                    ViewModel = new OkObjectResult(new UploadArquivoModel(output.Mensagem, output.Caminho, output.Retorno));
                    break;
                case UploadArquivoStatus.UploadNãoRealizado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
