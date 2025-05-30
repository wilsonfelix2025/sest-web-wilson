
using Microsoft.AspNetCore.Mvc;
using System.Net;
using SestWeb.Application.UseCases.TrendUseCases.CriarTrend;

namespace SestWeb.Api.UseCases.Trend.CriarTrend
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }


        internal void Populate(CriarTrendOutput output)
        {
            switch (output.Status)
            {
                case CriarTrendStatus.TrendCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Trend });
                    break;
                case CriarTrendStatus.TrendNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
