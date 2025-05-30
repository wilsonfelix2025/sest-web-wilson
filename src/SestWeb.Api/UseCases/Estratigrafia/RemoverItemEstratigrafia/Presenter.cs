using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.RemoverItemEstratigrafia;

namespace SestWeb.Api.UseCases.Estratigrafia.RemoverItemEstratigrafia
{
    /// Presenter do caso de uso Remover Item Estratigrafia
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
        internal void Populate(RemoverItemEstratigrafiaOutput output)
        {
            switch (output.Status)
            {
                case RemoverItemEstratigrafiaStatus.ItemEstratigrafiaRemovido:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverItemEstratigrafiaStatus.ItemEstratigrafiaNãoRemovido:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
