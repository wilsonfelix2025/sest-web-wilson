using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelaçãoPoço;

namespace SestWeb.Api.UseCases.Correlação.CriarCorrelaçãoPoço
{
    /// Presenter do caso de uso Criar Correlação do poço.
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
        internal void Populate(CriarCorrelaçãoPoçoOutput output)
        {
            switch (output.Status)
            {
                case CriarCorrelaçãoPoçoStatus.CorrelaçãoCriada:
                    ViewModel = new CreatedResult("", new { correlação = output.CorrelaçãoPoço.Correlação, status = output.Status, mensagem = output.Mensagem });
                    break;
                case CriarCorrelaçãoPoçoStatus.CorrelaçãoNãoCriada:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                case CriarCorrelaçãoPoçoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
