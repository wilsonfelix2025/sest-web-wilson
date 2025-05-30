using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PmPvUseCases;

namespace SestWeb.Api.UseCases.PmPv
{
    /// Presenter do caso de uso de conversão Pm/Pv
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
        internal void Populate(PmPvOutput output)
        {
            switch (output.Status)
            {
                case PmPvStatus.ConversãoRealizada:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PmPvStatus.ConversãoNãoRealizada:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
