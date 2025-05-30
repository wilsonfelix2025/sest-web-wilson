using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.AtualizarEstratigrafias;

namespace SestWeb.Api.UseCases.Estratigrafia.AtualizarEstratigrafias
{
    /// Presenter do caso de uso Adicionar Item Estratigrafia
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
        internal void Populate(AtualizarEstratigrafiasOutput output)
        {
            switch (output.Status)
            {
                case AtualizarEstratigrafiasStatus.Sucesso:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarEstratigrafiasStatus.Falha:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
