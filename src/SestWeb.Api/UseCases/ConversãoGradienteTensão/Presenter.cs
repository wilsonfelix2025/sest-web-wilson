using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ConversãoGradienteTensão;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.ConversãoGradienteTensão
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        /// <summary>
        /// Popula a ViewModel
        /// </summary>
        /// <param name="output">Resultado do caso de uso</param>
        internal void Populate(ConversãoOutput output)
        {
            switch (output.Status)
            {
                case ConversãoStatus.Convertido:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, perfil = output.Perfil });
                    break;
                case ConversãoStatus.NãoConvertido:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
