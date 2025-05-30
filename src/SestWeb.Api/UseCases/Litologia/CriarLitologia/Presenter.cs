using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.CriarLitologia;

namespace SestWeb.Api.UseCases.Litologia.CriarLitologia
{
    /// Presenter do caso de uso Criar litologia.
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
        internal void Populate(CriarLitologiaOutput output)
        {
            switch (output.Status)
            {
                case CriarLitologiaStatus.LitologiaCriada:
                    ViewModel = new CreatedResult("", output.Mensagem);
                    break;
                case CriarLitologiaStatus.LitologiaNãoCriada:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
