using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.ObterPoço;

namespace SestWeb.Api.UseCases.Poço.ObterPoço
{
    /// Presenter do caso de uso Obter Poço
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
        internal void Populate(ObterPoçoOutput output)
        {
            switch (output.Status)
            {
                case ObterPoçoStatus.PoçoObtido:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, poço = output.Poço, caminho = output.Caminho });
                    break;
                case ObterPoçoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterPoçoStatus.PoçoNãoObtido:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
