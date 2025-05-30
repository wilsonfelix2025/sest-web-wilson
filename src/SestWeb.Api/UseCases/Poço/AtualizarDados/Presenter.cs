using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados;

namespace SestWeb.Api.UseCases.Poço.AtualizarDados
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
        internal void Populate(AtualizarDadosOutput output)
        {
            switch (output.Status)
            {
                case AtualizarDadosStatus.DadosAtualizados:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, output.PerfisAlterados, output.Litologias });
                    break;
                case AtualizarDadosStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarDadosStatus.DadosNãoAtualizados:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
