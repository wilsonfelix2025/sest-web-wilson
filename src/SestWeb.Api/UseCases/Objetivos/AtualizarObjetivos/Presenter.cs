using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ObjetivoUseCases.AtualizarObjetivos;
using System.Net;

namespace SestWeb.Api.UseCases.Objetivos.AtualizarObjetivos
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
        internal void Populate(AtualizarObjetivosOutput output)
        {
            switch (output.Status)
            {
                case AtualizarObjetivosStatus.Sucesso:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarObjetivosStatus.Falha:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
