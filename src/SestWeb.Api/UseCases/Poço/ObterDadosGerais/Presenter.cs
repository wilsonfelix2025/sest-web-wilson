using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoUseCases.ObterDadosGerais;

namespace SestWeb.Api.UseCases.Poço.ObterDadosGerais
{
    /// Presenter do caso de uso Obter Dados Gerais
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
        internal void Populate(ObterDadosGeraisOutput output)
        {
            switch (output.Status)
            {
                case ObterDadosGeraisStatus.DadosGeraisObtidos:
                    ViewModel = new OkObjectResult(output.DadosGerais);
                    break;
                case ObterDadosGeraisStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(output.Mensagem);
                    break;
                case ObterDadosGeraisStatus.DadosGeraisNãoObtidos:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}