using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMec
{
    /// Presenter do caso de uso criar relacionamento corr prop mec do usuário
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
        internal void Populate(CriarRelacionamentoCorrsPropMecOutput output)
        {
            switch (output.Status)
            {
                case CriarRelacionamentoCorrsPropMecStatus.RelacionamentoCriado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, relacionamentos = output.RelacionamentoPropMec });
                    break;
                case CriarRelacionamentoCorrsPropMecStatus.RelacionamentoNãoCriado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case CriarRelacionamentoCorrsPropMecStatus.CorrelaçãoInexistente:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case CriarRelacionamentoCorrsPropMecStatus.RelacionamentoExistente:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case CriarRelacionamentoCorrsPropMecStatus.PoçoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
