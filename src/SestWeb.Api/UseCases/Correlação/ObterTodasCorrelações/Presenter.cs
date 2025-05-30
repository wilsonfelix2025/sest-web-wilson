using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrelações;

namespace SestWeb.Api.UseCases.Correlação.ObterTodasCorrelações
{
    /// Presenter do caso de uso Obter todas correlações
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
        internal void Populate(ObterTodasCorrelaçõesOutput output)
        {
            switch (output.Status)
            {
                case ObterTodasCorrelaçõesStatus.CorrelaçõesObtidas:
                    var viewModels = new List<ObterTodasCorrelaçõesViewModel>();
                    viewModels.AddRange(output.Correlações.Select(corr => new ObterTodasCorrelaçõesViewModel(corr)));
                    ViewModel = new OkObjectResult(viewModels);
                    break;
                case ObterTodasCorrelaçõesStatus.CorrelaçõesNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterTodasCorrelaçõesStatus.CorrelaçõesNãoEncontradas:
                    ViewModel= new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterTodasCorrelaçõesStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
