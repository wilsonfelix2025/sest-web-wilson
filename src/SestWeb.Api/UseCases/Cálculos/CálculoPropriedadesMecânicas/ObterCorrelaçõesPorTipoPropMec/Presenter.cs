using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPorTipoPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterCorrelaçõesPorTipoPropMec
{
    /// Presenter do caso de uso de obtenção das correlações do mnemônico informado (ucs ou coesa ou angat ou restr) que estão em relacionamentos do grupoLitológico informado.
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
        internal void Populate(ObterCorrelaçõesPorTipoPropMecOutput output)
        {
            switch (output.Status)
            {
                case ObterCorrelaçõesPorTipoPropMecStatus.CorrelaçõesObtidas:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, correlações = output.Correlações });
                    break;
                case ObterCorrelaçõesPorTipoPropMecStatus.GrupoLitológicoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPorTipoPropMecStatus.CorrelaçõesNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPorTipoPropMecStatus.PoçoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
