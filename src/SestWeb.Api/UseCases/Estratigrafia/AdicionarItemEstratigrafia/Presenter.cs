using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.AdicionarItemEstratigrafia;

namespace SestWeb.Api.UseCases.Estratigrafia.AdicionarItemEstratigrafia
{
    /// Presenter do caso de uso Adicionar Item Estratigrafia
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
        internal void Populate(AdicionarItemEstratigrafiaOutput output)
        {
            switch (output.Status)
            {
                case AdicionarItemEstratigrafiaStatus.ItemAdicionado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AdicionarItemEstratigrafiaStatus.ItemNãoAdicionado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
