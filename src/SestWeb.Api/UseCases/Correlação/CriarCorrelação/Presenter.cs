using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelação;

namespace SestWeb.Api.UseCases.Correlação.CriarCorrelação
{
    /// Presenter do caso de uso Criar Correlação
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
        internal void Populate(CriarCorrelaçãoOutput output)
        {
            switch (output.Status)
            {
                case CriarCorrelaçãoStatus.CorrelaçãoCriada:
                    ViewModel = new CreatedResult("", new { correlação = output.Correlação, status = output.Status, mensagem = output.Mensagem });
                    break;
                case CriarCorrelaçãoStatus.CorrelaçãoNãoCriada:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                case CriarCorrelaçãoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
