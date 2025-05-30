using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.RemoverCálculo;

namespace SestWeb.Api.UseCases.Cálculos.RemoverCálculo
{
    /// Presenter do caso de uso Remover Perfil
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
        public void Populate(RemoverCálculoOutput output)
        {
            switch (output.Status)
            {
                case RemoverCálculoStatus.CálculoRemovido:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverCálculoStatus.CálculoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverCálculoStatus.CálculoNãoRemovido:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}