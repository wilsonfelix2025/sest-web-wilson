using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.CreateOilField;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.CreateOilField
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(CreateOilFieldOutput output)
        {
            switch(output.Status)
            {
                case CreateOilFieldStatus.OilFieldCreated:
                    ViewModel = new CreatedResult("", new { name = output.Name, opunit = output.OpUnit, status = output.Status, mensagem = output.Mensagem });
                    break;
                case CreateOilFieldStatus.OilFieldNotCreated:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
