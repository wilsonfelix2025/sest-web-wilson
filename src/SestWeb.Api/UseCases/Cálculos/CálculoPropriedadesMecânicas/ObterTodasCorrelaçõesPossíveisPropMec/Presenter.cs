using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodasCorrelaçõesPossíveisPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterTodasCorrelaçõesPossíveisPropMec
{
    /// Presenter do caso de uso de obtenção de todas correlações ucs, coesa, angat e restr que estão em relacionamentos do grupoLitológico informado.
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
        internal void Populate(ObterTodasCorrelaçõesPossíveisPropMecOutput output)
        {
            switch (output.Status)
            {
                case ObterTodasCorrelaçõesPossíveisPropMecStatus.CorrelaçõesObtidas:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, correlaçõesPossíveis = output.CorrelaçõesPossíveis });
                    break;
                case ObterTodasCorrelaçõesPossíveisPropMecStatus.CorrelaçõesNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterTodasCorrelaçõesPossíveisPropMecStatus.PoçoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
