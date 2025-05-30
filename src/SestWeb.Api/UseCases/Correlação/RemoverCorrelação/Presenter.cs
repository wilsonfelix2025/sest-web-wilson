using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelação;

namespace SestWeb.Api.UseCases.Correlação.RemoverCorrelação
{
    /// Presenter do caso de uso Remover Correlação
    public class Presenter
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        /// Popula a ViewModel
        /// </summary>
        /// <param name="output">Objeto a ser enviado para o cliente</param>
        public void Populate(RemoverCorrelaçãoOutput output)
        {
            switch (output.Status)
            {
                case RemoverCorrelaçãoStatus.CorrelaçãoRemovida:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverCorrelaçãoStatus.CorrelaçãoNãoEncontrada:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverCorrelaçãoStatus.CorrelaçãoNãoRemovida:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverCorrelaçãoStatus.CorrelaçãoSemPermissãoParaRemoção:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
