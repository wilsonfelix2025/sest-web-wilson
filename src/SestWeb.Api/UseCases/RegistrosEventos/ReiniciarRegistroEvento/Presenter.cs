using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.RegistrosEventosUseCases.ReiniciarRegistroEvento;

namespace SestWeb.Api.UseCases.RegistrosEventos.ReiniciarRegistroEvento
{
    /// Presenter do caso de uso Reiniciar Registro / Evento
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
        public void Populate(ReiniciarRegistroEventoOutput output)
        {
            switch (output.Status)
            {
                case ReiniciarRegistroEventoStatus.RegistroEventoReiniciado:
                    ViewModel = new NoContentResult();
                    break;
                case ReiniciarRegistroEventoStatus.RegistroEventoNãoReiniciado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ReiniciarRegistroEventoStatus.RegistroEventoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
