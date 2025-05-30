using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho;
using System.Net;

namespace SestWeb.Api.UseCases.Perfil.ObterPerfisParaTrecho
{
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
        internal void Populate(ObterPerfisTrechoOutput output)
        {
            switch (output.Status)
            {
                case ObterPerfisTrechoStatus.PerfisObtidos:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, dados = output.ObterPerfisTrechoModel });
                    break;
                case ObterPerfisTrechoStatus.PerfisNãoObtidos:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
