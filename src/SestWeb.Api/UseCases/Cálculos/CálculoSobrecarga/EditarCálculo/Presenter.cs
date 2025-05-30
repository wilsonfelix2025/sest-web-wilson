using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.EditarCálculo;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoSobrecarga.EditarCálculo
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(EditarCálculoSobrecargaOutput output)
        {
            switch (output.Status)
            {
                case EditarCálculoSobrecargaStatus.CálculoSobrecargaEditado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Cálculo, output.PerfisAlterados });
                    break;
                case EditarCálculoSobrecargaStatus.CálculoSobrecargaNãoEditado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
