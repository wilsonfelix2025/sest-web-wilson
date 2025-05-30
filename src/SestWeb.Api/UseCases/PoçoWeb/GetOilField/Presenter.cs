using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetOilField;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.GetOilField
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(GetOilFieldOutput output)
        {
            switch (output.Status)
            {
                case GetOilFieldStatus.OilFieldFound:
                    ViewModel = new CreatedResult("", new { oilfield = output.Oilfield, status = output.Status, mensagem = output.Mensagem });
                    break;
                case GetOilFieldStatus.OilFieldNotFound:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
