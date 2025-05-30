using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.LitologiaUseCases.EditarLitologia;

namespace SestWeb.Api.UseCases.Litologia.EditarLitologia
{
    /// Presenter do caso de uso Editar litologia.
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
        internal void Populate(EditarLitologiaOutput output)
        {
            switch (output.Status)
            {
                case EditarLitologiaStatus.LitologiaEditada:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, litologia = output.Litologia, output.PerfisAlterados });
                    break;
                case EditarLitologiaStatus.LitologiaNãoEditada:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                case EditarLitologiaStatus.LitologiaNãoEncontrada:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                case EditarLitologiaStatus.PoçoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
