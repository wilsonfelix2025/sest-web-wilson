using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelaçãoPoço;

namespace SestWeb.Api.UseCases.Correlação.RemoverCorrelaçãoPoço
{
    /// Presenter do caso de uso Remover Correlação do poço.
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
        public void Populate(RemoverCorrelaçãoPoçoOutput output)
        {
            switch (output.Status)
            {
                case RemoverCorrelaçãoPoçoStatus.CorrelaçãoRemovida:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverCorrelaçãoPoçoStatus.CorrelaçãoNãoEncontrada:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverCorrelaçãoPoçoStatus.CorrelaçãoNãoRemovida:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverCorrelaçãoPoçoStatus.CorrelaçãoSemPermissãoParaRemoção:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverCorrelaçãoPoçoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
