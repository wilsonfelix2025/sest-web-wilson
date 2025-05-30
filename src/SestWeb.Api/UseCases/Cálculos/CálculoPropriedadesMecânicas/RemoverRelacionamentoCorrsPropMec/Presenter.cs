using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMec
{
    /// Presenter do caso de uso Remover relacionamento de correlações prop mec do usuário.
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
        public void Populate(RemoverRelacionamentoCorrsPropMecOutput output)
        {
            switch (output.Status)
            {
                case RemoverRelacionamentoCorrsPropMecStatus.RelacionamentoRemovido:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverRelacionamentoCorrsPropMecStatus.RelacionamentoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverRelacionamentoCorrsPropMecStatus.RelacionamentoNãoRemovido:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverRelacionamentoCorrsPropMecStatus.RelacionamentoSemPermissãoParaRemoção:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
