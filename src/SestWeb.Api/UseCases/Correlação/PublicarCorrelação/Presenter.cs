using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.PublicarCorrelação;

namespace SestWeb.Api.UseCases.Correlação.PublicarCorrelação
{
    /// Presenter do caso de uso Publicar Correlação.
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
        internal void Populate(PublicarCorrelaçãoOutput output)
        {
            switch (output.Status)
            {
                case PublicarCorrelaçãoStatus.CorrelaçãoPublicada:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PublicarCorrelaçãoStatus.CorrelaçãoNãoEncontrada:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PublicarCorrelaçãoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PublicarCorrelaçãoStatus.CorrelaçãoNãoPublicada:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PublicarCorrelaçãoStatus.CorrelaçãoExistente:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
