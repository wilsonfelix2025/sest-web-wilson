using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterQtdPontosPerfil;

namespace SestWeb.Api.UseCases.Perfil.ObterQtdPontosPerfil
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
        internal void Populate(ObterQtdPontosPerfilOutput output)
        {
            switch (output.Status)
            {
                case ObterQtdPontosPerfilStatus.QtdObtida:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, qtd = output.Qtd });
                    break;
                case ObterQtdPontosPerfilStatus.QtdNãoObtida:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
