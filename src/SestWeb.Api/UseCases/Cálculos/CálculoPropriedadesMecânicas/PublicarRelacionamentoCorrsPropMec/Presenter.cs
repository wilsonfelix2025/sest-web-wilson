using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.PublicarRelacionamentoCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.PublicarRelacionamentoCorrsPropMec
{
    /// Presenter do caso de uso Publicar relacionamento..
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
        internal void Populate(PublicarRelacionamentoCorrsPropMecOutput output)
        {
            switch (output.Status)
            {
                case PublicarRelacionamentoCorrsPropMecStatus.RelacionamentoPublicado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PublicarRelacionamentoCorrsPropMecStatus.RelacionamentoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PublicarRelacionamentoCorrsPropMecStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PublicarRelacionamentoCorrsPropMecStatus.RelacionamentoNãoPublicado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case PublicarRelacionamentoCorrsPropMecStatus.RelacionamentoExistente:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
