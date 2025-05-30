using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.RenomearPoço;

namespace SestWeb.Api.UseCases.Poço.RenomearPoço
{
    /// Presenter do caso de uso Renomear Poço
    public class Presenter
    {
        /// <summary>
        ///     ViewModel
        /// </summary>
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        ///     Popula a ViewModel
        /// </summary>
        /// <param name="output">Resultado do caso de uso</param>
        internal void Populate(RenomearPoçoOutput output)
        {
            switch (output.Status)
            {
                case RenomearPoçoStatus.PoçoRenomeado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RenomearPoçoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RenomearPoçoStatus.PoçoNãoRenomeado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int) HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}