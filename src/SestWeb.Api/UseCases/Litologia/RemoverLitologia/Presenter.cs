using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.RemoverLitologia;

namespace SestWeb.Api.UseCases.Litologia.RemoverLitologia
{
    /// Presenter do caso de uso Remover litologia
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
        public void Populate(RemoverLitologiaOutput output)
        {
            switch (output.Status)
            {
                case RemoverLitologiaStatus.LitologiaRemovida:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverLitologiaStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(output.Mensagem);
                    break;
                case RemoverLitologiaStatus.LitologiaNãoRemovida:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
