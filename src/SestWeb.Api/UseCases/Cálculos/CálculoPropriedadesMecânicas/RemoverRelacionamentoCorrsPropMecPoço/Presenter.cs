using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMecPoço;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMecPoço
{
    /// Presenter do caso de uso Remover relacionamento do poço.
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
        public void Populate(RemoverRelacionamentoCorrsPropMecOutputPoço output)
        {
            switch (output.Status)
            {
                case RemoverRelacionamentoCorrsPropMecStatusPoço.RelacionamentoRemovido:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverRelacionamentoCorrsPropMecStatusPoço.RelacionamentoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverRelacionamentoCorrsPropMecStatusPoço.RelacionamentoNãoRemovido:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverRelacionamentoCorrsPropMecStatusPoço.RelacionamentoSemPermissãoParaRemoção:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case RemoverRelacionamentoCorrsPropMecStatusPoço.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
