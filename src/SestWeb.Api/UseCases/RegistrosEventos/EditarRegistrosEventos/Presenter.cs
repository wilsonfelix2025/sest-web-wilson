using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.RegistrosEventosUseCases.EditarRegistrosEventos;

namespace SestWeb.Api.UseCases.RegistrosEventos.EditarRegistrosEventos
{
    /// Presenter do caso de uso Editar Registros e Eventos
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
        internal void Populate(EditarRegistrosEventosOutput output)
        {
            switch (output.Status)
            {
                case EditarRegistrosEventosStatus.RegistrosEventosEditados:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, registrosEventos = output.RegistrosEventos });
                    break;
                case EditarRegistrosEventosStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case EditarRegistrosEventosStatus.RegistrosEventosNãoEditados:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
