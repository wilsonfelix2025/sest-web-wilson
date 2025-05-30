using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.AdicionarPoçoApoio;

namespace SestWeb.Api.UseCases.Poço.AdicionarPoçoApoio
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
        internal void Populate(AdicionarPoçoApoioOutput output)
        {
            switch (output.Status)
            {
                case AdicionarPoçoApoioStatus.ComSucesso:
                    ViewModel = new OkObjectResult(new { output.Mensagem });
                    break;
                case AdicionarPoçoApoioStatus.SemSucesso:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
