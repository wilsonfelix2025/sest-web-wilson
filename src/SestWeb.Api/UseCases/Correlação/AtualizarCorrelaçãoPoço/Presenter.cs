using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelaçãoPoço;

namespace SestWeb.Api.UseCases.Correlação.AtualizarCorrelaçãoPoço
{
    /// Presenter do caso de uso Atualizar Correlação do poço.
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
        internal void Populate(AtualizarCorrelaçãoPoçoOutput output)
        {
            switch (output.Status)
            {
                case AtualizarCorrelaçãoPoçoStatus.CorrelaçãoAtualizada:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarCorrelaçãoPoçoStatus.CorrelaçãoNãoEncontrada:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarCorrelaçãoPoçoStatus.PoçoNãoEncontrado:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case AtualizarCorrelaçãoPoçoStatus.CorrelaçãoNãoAtualizada:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
