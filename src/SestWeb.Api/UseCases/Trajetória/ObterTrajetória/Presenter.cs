using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrajetóriaUseCases.ObterTrajetória;

namespace SestWeb.Api.UseCases.Trajetória.ObterTrajetória
{
    /// Presenter do caso de uso Obter trajetória
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
        internal void Populate(ObterTrajetóriaOutput output)
        {
            switch (output.Status)
            {
                case ObterTrajetóriaStatus.TrajetóriaObtida:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, trajetória = output.Trajetória });
                    break;
                case ObterTrajetóriaStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterTrajetóriaStatus.TrajetóriaNãoObtida:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
