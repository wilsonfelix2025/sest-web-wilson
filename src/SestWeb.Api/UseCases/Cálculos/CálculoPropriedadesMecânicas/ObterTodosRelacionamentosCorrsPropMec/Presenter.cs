using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodosRelacionamentosCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterTodosRelacionamentosCorrsPropMec
{
    /// Presenter do caso de uso Obter todos relacionamentos
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
        internal void Populate(ObterTodosRelacionamentosCorrsPropMecOutput output)
        {
            switch (output.Status)
            {
                case ObterTodosRelacionamentosCorrsPropMecStatus.RelacionamentosObtidos:
                    ViewModel = new OkObjectResult(output.Relacionamentos);
                    break;
                case ObterTodosRelacionamentosCorrsPropMecStatus.RelacionamentosNãoObtidos:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterTodosRelacionamentosCorrsPropMecStatus.RelacionamentosNãoEncontrados:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterTodosRelacionamentosCorrsPropMecStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
