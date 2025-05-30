using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.EditarCálculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPressãoPoros.EditarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(EditarCálculoPressãoPorosOutput output)
        {
            switch (output.Status)
            {
                case EditarCálculoPressãoPorosStatus.CálculoPressãoPorosEditado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo, output.PerfisAlterados });
                    break;
                case EditarCálculoPressãoPorosStatus.CálculoPressãoPorosNãoEditado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
