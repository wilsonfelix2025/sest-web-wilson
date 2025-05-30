using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisDeUmPoço;

namespace SestWeb.Api.UseCases.Perfil.ObterPerfisDeUmPoço
{
    /// Presenter do caso de uso Obter perfis de um poço
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
        internal void Populate(ObterPerfisDeUmPoçoOutput output)
        {
            switch (output.Status)
            {
                case ObterPerfisDeUmPoçoStatus.PerfisObtidos:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, perfis = output.Perfis });
                    break;
                case ObterPerfisDeUmPoçoStatus.PerfisNãoObtidos:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
