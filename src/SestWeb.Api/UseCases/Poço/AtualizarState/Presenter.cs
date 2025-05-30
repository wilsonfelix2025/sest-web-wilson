using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarState;

namespace SestWeb.Api.UseCases.Poço.AtualizarState
{
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
        internal void Populate(AtualizarStateOutput output)
        {
            switch (output.Status)
            {
                case AtualizarStateStatus.StateAtualizado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, state = output.State });
                    break;
                case AtualizarStateStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarStateStatus.StateNãoAtualizado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
