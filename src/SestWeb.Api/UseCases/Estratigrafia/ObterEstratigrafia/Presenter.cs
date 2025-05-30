using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.ObterEstratigrafia;

namespace SestWeb.Api.UseCases.Estratigrafia.ObterEstratigrafia
{
    /// Presenter do caso de uso Obter estratigrafia
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
        internal void Populate(ObterEstratigrafiaOutput output)
        {
            switch (output.Status)
            {
                case ObterEstratigrafiaStatus.EstratigrafiaObtida:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, estratigrafia = output.Estratigrafia });
                    break;
                case ObterEstratigrafiaStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterEstratigrafiaStatus.EstratigrafiaNãoObtida:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
