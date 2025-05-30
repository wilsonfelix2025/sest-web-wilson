using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.RemoverPerfil;

namespace SestWeb.Api.UseCases.Perfil.RemoverPerfil
{
    /// Presenter do caso de uso Remover Perfil
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
        public void Populate(RemoverPerfilOutput output)
        {
            switch (output.Status)
            {
                case RemoverPerfilStatus.PerfilRemovido:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverPerfilStatus.PerfilNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverPerfilStatus.PerfilNãoRemovido:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
