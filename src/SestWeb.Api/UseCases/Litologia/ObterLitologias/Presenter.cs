using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologias;

namespace SestWeb.Api.UseCases.Litologia.ObterLitologias
{
    /// Presenter do caso de uso Obter litologias
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
        internal void Populate(ObterLitologiasOutput output)
        {
            switch (output.Status)
            {
                case ObterLitologiasStatus.LitologiasObtidas:
                    ViewModel = new OkObjectResult(output.Litologias);
                    break;
                case ObterLitologiasStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(output.Mensagem);
                    break;
                case ObterLitologiasStatus.LitologiasNãoObtidas:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
