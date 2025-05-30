using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrendUseCases.EditarTrend;

namespace SestWeb.Api.UseCases.Trend.EditarTrend
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }


        internal void Populate(EditarTrendOutput output)
        {
            switch (output.Status)
            {
                case EditarTrendStatus.TrendEditado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Trend, output.PerfisAlterados });
                    break;
                case EditarTrendStatus.TrendNãoEditado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
