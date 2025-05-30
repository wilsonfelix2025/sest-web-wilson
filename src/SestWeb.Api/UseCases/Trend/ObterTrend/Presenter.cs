using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrendUseCases.ObterTrend;

namespace SestWeb.Api.UseCases.Trend.ObterTrend
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }


        internal void Populate(ObterTrendOutput output)
        {
            switch (output.Status)
            {
                case ObterTrendStatus.TrendObtido:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Trend });
                    break;
                case ObterTrendStatus.TrendNãoObtido:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
