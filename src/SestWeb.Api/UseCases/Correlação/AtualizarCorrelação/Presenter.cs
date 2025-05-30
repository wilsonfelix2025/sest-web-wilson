using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelação;

namespace SestWeb.Api.UseCases.Correlação.AtualizarCorrelação
{
    /// Presenter do caso de uso Atualizar Correlação
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
        internal void Populate(AtualizarCorrelaçãoOutput output)
        {
            switch (output.Status)
            {
                case AtualizarCorrelaçãoStatus.CorrelaçãoAtualizada:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarCorrelaçãoStatus.CorrelaçãoNãoEncontrada:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarCorrelaçãoStatus.CorrelaçãoNãoAtualizada:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
