using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.LoadRelacionamentosCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.LoadRelacionamentosCorrsPropMec
{
    /// Presenter do caso de uso Load relacionamentos corrs prop mec Sistema
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
        internal void Populate(LoadRelacionamentosCorrsPropMecOutput output)
        {
            switch (output.Status)
            {
                case LoadRelacionamentosCorrsPropMecStatus.RelacionamentosCarregados:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, relacionamentos = output.Relacionamentos });
                    break;
                case LoadRelacionamentosCorrsPropMecStatus.RelacionamentosNãoCarregados:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case LoadRelacionamentosCorrsPropMecStatus.RelacionamentosJáCarregados:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case LoadRelacionamentosCorrsPropMecStatus.CorrelaçõesNãoEncontradas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case LoadRelacionamentosCorrsPropMecStatus.PoçoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case LoadRelacionamentosCorrsPropMecStatus.RelacionamentosNãoEncontrados:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
