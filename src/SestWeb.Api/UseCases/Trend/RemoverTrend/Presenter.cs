using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrendUseCases.RemoverTrend;
using System.Net;

namespace SestWeb.Api.UseCases.Trend.RemoverTrend
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }


        internal void Populate(RemoverTrendOutput output)
        {
            switch (output.Status)
            {
                case RemoverTrendStatus.TrendRemovido:
                    ViewModel = new NoContentResult();
                    break;
                case RemoverTrendStatus.TrendNãoRemovido:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
