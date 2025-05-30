using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfil;

namespace SestWeb.Api.UseCases.Perfil.ObterPerfil
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
        internal void Populate(ObterPerfilOutput output)
        {
            switch (output.Status)
            {
                case ObterPerfilStatus.PerfilObtido:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, perfil = output.PerfilOld });
                    break;
                case ObterPerfilStatus.PerfilNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new {mensagem = output.Mensagem});
                    break;
                case ObterPerfilStatus.PerfilNãoObtido:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
