using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrajetóriaUseCases.EditarTrajetória;

namespace SestWeb.Api.UseCases.Trajetória.EditarTrajetória
{
    /// Presenter do caso de uso de editar trajetória
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
        internal void Populate(EditarTrajetóriaOutput output)
        {
            switch (output.Status)
            {
                case EditarTrajetóriaStatus.TrajetóriaEditada:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, Trajetória = output.Trajetória, output.PerfisAlterados, output.Litologias });
                    break;
                case EditarTrajetóriaStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new {mensagem = output.Mensagem});
                    break;
                case EditarTrajetóriaStatus.TrajetóriaNãoEditada:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
