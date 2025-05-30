using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.CriarPerfil;

namespace SestWeb.Api.UseCases.Perfil.CriarPerfil
{
    /// Presenter do caso de uso Criar Perfil
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
        internal void Populate(CriarPerfilOutput output)
        {
            switch (output.Status)
            {
                case CriarPerfilStatus.PerfilCriado:
                    ViewModel = new CreatedAtRouteResult("ObterPerfil", new {id = output.PerfilOld.Id}, output.PerfilOld);
                    break;
                case CriarPerfilStatus.PerfilNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int) HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
