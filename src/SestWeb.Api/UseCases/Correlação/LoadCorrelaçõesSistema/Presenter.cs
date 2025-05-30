using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.LoadCorrelaçõesSistema;

namespace SestWeb.Api.UseCases.Correlação.LoadCorrelaçõesSistema
{
    /// Presenter do caso de uso Load Correlações Sistema
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
        internal void Populate(LoadCorrelaçõesSistemaOutput output)
        {
            switch (output.Status)
            {
                case LoadCorrelaçõesSistemaStatus.CorrelaçõesCarregadas:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, correlações = output.Correlações });
                    break;
                case LoadCorrelaçõesSistemaStatus.CorrelaçõesNãoCarregadas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                case LoadCorrelaçõesSistemaStatus.CorrelaçõesJáCarregadas:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
