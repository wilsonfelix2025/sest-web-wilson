using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.CreateWell;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.CreateWell
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(CreateWellOutput output)
        {
            switch (output.Status)
            {
                case CreateWellStatus.WellCreated:
                    ViewModel = new CreatedResult("", new { name = output.Name, id = output.Id, oilfield = output.Oilfield, status = output.Status, mensagem = output.Mensagem });
                    break;
                case CreateWellStatus.WellNotCreated:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
