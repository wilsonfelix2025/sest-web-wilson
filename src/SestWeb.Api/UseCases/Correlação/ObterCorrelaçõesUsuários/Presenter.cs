using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesUsuários;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelaçõesUsuários
{
    /// Presenter do caso de uso Obter todas correlações de usuários.
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
        internal void Populate(ObterCorrelaçõesUsuáriosOutput output)
        {
            switch (output.Status)
            {
                case ObterCorrelaçõesUsuáriosStatus.CorrelaçõesObtidas:
                    var viewModels = new List<ObterCorrelaçõesUsuáriosViewModel>();
                    viewModels.AddRange(output.Correlações.Select(corr => new ObterCorrelaçõesUsuáriosViewModel(corr)));
                    ViewModel = new OkObjectResult(viewModels);
                    break;
                case ObterCorrelaçõesUsuáriosStatus.CorrelaçõesNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesUsuáriosStatus.CorrelaçõesNãoEncontradas:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesUsuáriosStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
