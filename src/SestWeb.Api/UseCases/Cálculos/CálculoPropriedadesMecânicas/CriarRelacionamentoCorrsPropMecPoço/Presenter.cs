using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço
{
    /// Presenter do caso de uso criar relacionamento corr prop mec poço
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
        internal void Populate(CriarRelacionamentoCorrsPropMecPoçoOutput output)
        {
            switch (output.Status)
            {
                case CriarRelacionamentoCorrsPropMecPoçoStatus.RelacionamentoCriado:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, relacionamentos = output.RelacionamentoPropMecPoço });
                    break;
                case CriarRelacionamentoCorrsPropMecPoçoStatus.RelacionamentoNãoCriado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case CriarRelacionamentoCorrsPropMecPoçoStatus.CorrelaçãoInexistente:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case CriarRelacionamentoCorrsPropMecPoçoStatus.RelacionamentoExistente:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case CriarRelacionamentoCorrsPropMecPoçoStatus.PoçoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case CriarRelacionamentoCorrsPropMecPoçoStatus.GrupoLitológicoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
