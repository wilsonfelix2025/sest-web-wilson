using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçãoPeloNome;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelação
{
    /// Presenter do caso de uso Obter correlação
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
        internal void Populate(ObterCorrelaçãoPeloNomeOutput output)
        {
            switch (output.Status)
            {
                case ObterCorrelaçãoPeloNomeStatus.CorrelaçãoObtida:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, nome = output.Nome, perfilSaída = output.PerfilSaída, expressão = output.Expressão, chaveAutor = output.ChaveAutor, descrição = output.Descrição });
                    break;
                case ObterCorrelaçãoPeloNomeStatus.CorrelaçãoNãoEncontrada:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçãoPeloNomeStatus.CorrelaçãoNãoObtida:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçãoPeloNomeStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
