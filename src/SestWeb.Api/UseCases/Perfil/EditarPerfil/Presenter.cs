using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.EditarPerfil;

namespace SestWeb.Api.UseCases.Perfil.EditarPerfil
{
    /// Presenter do caso de uso Obter perfil
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
        internal void Populate(EditarPerfilOutput output)
        {
            switch (output.Status)
            {
                case EditarPerfilStatus.PerfilEditado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, perfil = output.Perfil, output.PerfisAlterados });
                    break;
                case EditarPerfilStatus.PerfilNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new {mensagem = output.Mensagem});
                    break;
                case EditarPerfilStatus.PerfilNãoEditado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
