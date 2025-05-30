using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterAutoresCorrelações;

namespace SestWeb.Api.UseCases.Correlação.ObterAutores
{
    /// Presenter do caso de uso Obter autores de correlações
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
        internal void Populate(ObterAutoresCorrelaçõesOutput output)
        {
            switch (output.Status)
            {
                case ObterAutoresCorrelaçõesStatus.AutoresObtidos:
                    var viewModels = new List<ObterAutoresViewModel>();
                    viewModels.AddRange(output.Autores.Select(aut => new ObterAutoresViewModel(aut)));
                    ViewModel = new OkObjectResult(viewModels);
                    break;
                case ObterAutoresCorrelaçõesStatus.AutoresNãoObtidos:
                    ViewModel = new BadRequestObjectResult(new {mensagem = output.Mensagem});
                    break;
                case ObterAutoresCorrelaçõesStatus.AutoresNãoEncontrados:
                    ViewModel = new NotFoundObjectResult(new {mensagem = output.Mensagem});
                    break;
                case ObterAutoresCorrelaçõesStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int) HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
