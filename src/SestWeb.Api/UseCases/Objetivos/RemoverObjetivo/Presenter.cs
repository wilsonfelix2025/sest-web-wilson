using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ObjetivoUseCases.RemoverObjetivo;

namespace SestWeb.Api.UseCases.Objetivos.RemoverObjetivo
{
    /// Presenter do caso de uso Remover objetivo
    public class Presenter
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        /// Popula a ViewModel
        /// </summary>
        /// <param name="output">Objeto a ser enviado para o cliente</param>
        public void Populate(RemoverObjetivoOutput output)
        {
            switch (output.Status)
            {
                case RemoverObjetivoStatus.ObjetivoRemovido:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverObjetivoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(output.Mensagem);
                    break;
                case RemoverObjetivoStatus.ObjetivoNãoRemovido:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
