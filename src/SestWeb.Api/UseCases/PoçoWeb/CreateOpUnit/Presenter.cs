using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.CreateOpUnit;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.CreateOpUnit
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        public void Populate(CreateOpUnitOutput output)
        {
            switch(output.Status)
            {
                case CreateOpUnitStatus.OpUnitCreated:
                    ViewModel = new CreatedResult("", new { name = output.Name, status = output.Status, mensagem = output.Mensagem });
                    break;
                case CreateOpUnitStatus.OpUnitNotCreated:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
