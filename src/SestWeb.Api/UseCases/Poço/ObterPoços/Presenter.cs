using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.ObterPoços;

namespace SestWeb.Api.UseCases.Poço.ObterPoços
{
    /// Presenter do caso de uso Obter Poços
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
        internal void Populate(ObterPoçosOutput output)
        {
            switch (output.Status)
            {
                case ObterPoçosStatus.PoçosObtidos:
                    var viewModels = new List<ObterPoçosViewModel>();
                    viewModels.AddRange(output.Poços.Select(poço => new ObterPoçosViewModel(poço)));
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, poços = viewModels });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
