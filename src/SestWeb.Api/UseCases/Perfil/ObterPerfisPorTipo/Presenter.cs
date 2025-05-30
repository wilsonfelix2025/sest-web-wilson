using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisPorTipo;

namespace SestWeb.Api.UseCases.Perfil.ObterPerfisPorTipo
{
    /// Presenter do caso de uso Obter perfis por tipo
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
        internal void Populate(ObterPerfisPorTipoOutput output)
        {
            switch (output.Status)
            {
                case ObterPerfisPorTipoStatus.PerfisObtidos:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, perfis = output.Perfis });
                    break;
                case ObterPerfisPorTipoStatus.PerfisNãoObtidos:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
