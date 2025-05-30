using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ObjetivoUseCases.CriarObjetivo;

namespace SestWeb.Api.UseCases.Objetivos.CriarObjetivo
{
    /// Presenter do caso de uso Criar objetivo.
    public class Presenter
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        /// Popula a ViewModel.
        /// </summary>
        /// <param name="output">Resultado do caso de uso.</param>
        internal void Populate(CriarObjetivoOutput output)
        {
            switch (output.Status)
            {
                case CriarObjetivoStatus.ObjetivoCriado:
                    ViewModel = new CreatedResult("", output.Mensagem);
                    break;
                case CriarObjetivoStatus.ObjetivoNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int) HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}