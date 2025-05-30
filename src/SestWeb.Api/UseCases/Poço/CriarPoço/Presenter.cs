using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.CriarPoço;

namespace SestWeb.Api.UseCases.Poço.CriarPoço
{
    /// Presenter do caso de uso Criar Poço
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
        internal void Populate(CriarPoçoOutput output)
        {
            switch (output.Status)
            {
                case CriarPoçoStatus.PoçoCriado:
                    ViewModel = new CreatedAtRouteResult("ObterPoço", new { id = output.Poço.Id }, output.Poço);
                    break;
                case CriarPoçoStatus.PoçoNãoCriado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
