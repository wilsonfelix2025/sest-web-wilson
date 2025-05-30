using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.SapataUseCases.RemoverSapata;

namespace SestWeb.Api.UseCases.Sapatas.RemoverSapata
{
    /// Presenter do caso de uso Remover sapata
    public class Presenter
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        /// Popula a ViewModel
        /// </summary>
        /// <param name="output">Objeto a ser enviado para o cliente</param>
        public void Populate(RemoverSapataOutput output)
        {
            switch (output.Status)
            {
                case RemoverSapataStatus.SapataRemovida:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverSapataStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(output.Mensagem);
                    break;
                case RemoverSapataStatus.SapataNãoRemovida:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
