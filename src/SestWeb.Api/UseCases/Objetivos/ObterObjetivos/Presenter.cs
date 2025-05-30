using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ObjetivoUseCases.ObterObjetivos;

namespace SestWeb.Api.UseCases.Objetivos.ObterObjetivos
{
    /// <summary>
    /// Presenter do caso de uso Obter objetivos.
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
        internal void Populate(ObterObjetivosOutput output)
        {
            switch (output.Status)
            {
                case ObterObjetivosStatus.ObjetivosObtidos:
                    var viewModels = new List<ObterObjetivosViewModel>();
                    viewModels.AddRange(output.Objetivos.Select(objetivo => new ObterObjetivosViewModel(objetivo)));
                    ViewModel = new OkObjectResult(viewModels);
                    break;
                case ObterObjetivosStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(output.Mensagem);
                    break;
                case ObterObjetivosStatus.ObjetivosNãoObtidos:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
