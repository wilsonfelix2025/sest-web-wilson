using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorChaveUsuário;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelaçõesPorChaveUsuário
{
    /// Presenter do caso de uso Obter correlações por chave do usuário.
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
        internal void Populate(ObterCorrelaçõesPorChaveUsuárioOutput output)
        {
            switch (output.Status)
            {
                case ObterCorrelaçõesPorChaveUsuárioStatus.CorrelaçõesObtidas:
                    var viewModels = new List<ObterCorrelaçõesPorChaveUsuárioViewModel>();
                    viewModels.AddRange(output.Correlações.Select(corr => new ObterCorrelaçõesPorChaveUsuárioViewModel(corr)));
                    ViewModel = new OkObjectResult(viewModels);
                    break;
                case ObterCorrelaçõesPorChaveUsuárioStatus.CorrelaçõesNãoEncontradas:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPorChaveUsuárioStatus.CorrelaçõesNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPorChaveUsuárioStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
