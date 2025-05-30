using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologia;

namespace SestWeb.Api.UseCases.Litologia.ObterLitologia
{
    /// Presenter do caso de uso Obter litologia
    public class Presenter
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        /// Popula a ViewModel.
        /// </summary>
        /// <param name="output">Resultado do caso de uso.</param>
        internal void Populate(ObterLitologiaOutput output)
        {
            switch (output.Status)
            {
                case ObterLitologiaStatus.LitologiaObtida:
                    ViewModel = new OkObjectResult(output.Litologia);
                    break;
                case ObterLitologiaStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(output.Mensagem);
                    break;
                case ObterLitologiaStatus.LitologiaNãoObtida:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}