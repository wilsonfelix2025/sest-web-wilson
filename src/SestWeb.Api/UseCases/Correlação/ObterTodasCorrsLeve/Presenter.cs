using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrsLeve;

namespace SestWeb.Api.UseCases.Correlação.ObterTodasCorrsLeve
{
    /// Presenter do caso de uso Obter todas correlações leve
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
        internal void Populate(ObterTodasCorrsLeveOutput output)
        {
            switch (output.Status)
            {
                case ObterTodasCorrsLeveStatus.CorrelaçõesObtidas:
                    var viewModels = new List<ObterTodasCorrsLeveViewModel>();
                    viewModels.AddRange(output.Correlações.Select(corr => new ObterTodasCorrsLeveViewModel(corr)));
                    ViewModel = new OkObjectResult(viewModels);
                    break;
                case ObterTodasCorrsLeveStatus.CorrelaçõesNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterTodasCorrsLeveStatus.CorrelaçõesNãoEncontradas:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterTodasCorrsLeveStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
