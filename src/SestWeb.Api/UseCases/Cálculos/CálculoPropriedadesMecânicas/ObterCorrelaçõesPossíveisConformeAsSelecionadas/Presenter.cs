using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPossíveisConformeAsSelecionadas;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterCorrelaçõesPossíveisConformeAsSelecionadas
{
    /// Presenter do caso de uso de obtenção das correlações possíveis diante das correlações já selecionadas.
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
        internal void Populate(ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput output)
        {
            switch (output.Status)
            {
                case ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.CorrelaçõesObtidas:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, correlaçõesPossíveis = output.CorrelaçõesPossíveis });
                    break;
                case ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.CorrelaçãoNãoEncontrada:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.GrupoLitológicoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.CorrelaçõesNãoObtidas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.PoçoNãoEncontrado:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
