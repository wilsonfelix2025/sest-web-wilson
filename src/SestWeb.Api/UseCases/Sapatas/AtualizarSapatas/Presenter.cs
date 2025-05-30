using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.SapataUseCases.AtualizarSapatas;

namespace SestWeb.Api.UseCases.Sapatas.AtualizarSapatas
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
        internal void Populate(AtualizarSapatasOutput output)
        {
            switch (output.Status)
            {
                case AtualizarSapatasStatus.Sucesso:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarSapatasStatus.Falha:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }

    }
}
