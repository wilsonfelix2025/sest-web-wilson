using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.RemoverPoçoApoio;

namespace SestWeb.Api.UseCases.Poço.RemoverPoçoApoio
{
    /// Presenter do caso de uso Criar Poço
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
        internal void Populate(RemoverPoçoApoioOutput output)
        {
            switch (output.Status)
            {
                case RemoverPoçoApoioStatus.ComSucesso:
                    ViewModel = new OkObjectResult(new { output.Mensagem });
                    break;
                case RemoverPoçoApoioStatus.SemSucesso:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
