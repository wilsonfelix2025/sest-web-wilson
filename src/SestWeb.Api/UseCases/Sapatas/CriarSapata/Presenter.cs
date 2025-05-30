using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.SapataUseCases.CriarSapata;

namespace SestWeb.Api.UseCases.Sapatas.CriarSapata
{
    /// Presenter do caso de uso Criar Sapata
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
        internal void Populate(CriarSapataOutput output)
        {
            switch (output.Status)
            {
                case CriarSapataStatus.SapataCriada:
                    ViewModel = new CreatedResult("", output.Mensagem);
                    break;
                case CriarSapataStatus.SapataNãoCriada:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
