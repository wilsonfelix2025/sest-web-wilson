using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterTiposDePerfil;
using System.Net;

namespace SestWeb.Api.UseCases.Perfil.ObterTiposDePerfil
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
        internal void Populate(ObterTiposDePerfilOutput output)
        {
            switch (output.Status)
            {
                case ObterTiposDePerfilStatus.TiposDePerfilObtido:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, tiposDePerfil = output.TiposDePerfil });
                    break;                
                case ObterTiposDePerfilStatus.TiposDePerfilNãoObtido:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
