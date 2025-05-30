using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.SapataUseCases.ObterSapatas;

namespace SestWeb.Api.UseCases.Sapatas.ObterSapatas
{
    /// <summary>
    /// Presenter do caso de uso Obter sapatas
    /// </summary>
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
        internal void Populate(ObterSapatasOutput output)
        {
            switch (output.Status)
            {
                case ObterSapatasStatus.SapatasObtidas:
                    var viewModels = new List<ObterSapatasViewModel>();
                    viewModels.AddRange(output.Sapatas.Select(sapata => new ObterSapatasViewModel(sapata)));
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, sapatas = viewModels });
                    break;
                case ObterSapatasStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterSapatasStatus.SapatasNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
