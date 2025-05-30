using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.RemoverPoço;

namespace SestWeb.Api.UseCases.Poço.RemoverPoço
{
    /// Presenter do caso de uso Obter Poço
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
        public void Populate(RemoverPoçoOutput output)
        {
            switch (output.Status)
            {
                case RemoverPoçoStatus.PoçoRemovido:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverPoçoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(output.Mensagem);
                    break;
                case RemoverPoçoStatus.PoçoNãoRemovido:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}