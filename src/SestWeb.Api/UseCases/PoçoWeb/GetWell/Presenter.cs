using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetWell;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.GetWell
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(GetWellOutput output)
        {
            switch (output.Status)
            {
                case GetWellStatus.WellFound:
                    ViewModel = new CreatedResult("", new { oilfield = output.Well, status = output.Status, mensagem = output.Mensagem });
                    break;
                case GetWellStatus.WellNotFound:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
