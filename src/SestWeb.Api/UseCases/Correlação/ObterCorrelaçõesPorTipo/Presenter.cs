using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorTipo;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelaçõesPorTipo
{
    /// Presenter do caso de uso Obter correlações por tipo
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
        internal void Populate(ObterCorrelaçõesPorTipoOutput output)
        {
            switch (output.Status)
            {
                case ObterCorrelaçõesPorTipoStatus.CorrelaçõesObtidas:
                    var viewModels = new List<ObterCorrelaçõesPorTipoViewModel>();
                    viewModels.AddRange(output.Correlações.Select(corr => new ObterCorrelaçõesPorTipoViewModel(corr)));
                    ViewModel = new OkObjectResult(viewModels);
                    break;
                case ObterCorrelaçõesPorTipoStatus.CorrelaçõesNãoEncontradas:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPorTipoStatus.CorrelaçõesNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPorTipoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
