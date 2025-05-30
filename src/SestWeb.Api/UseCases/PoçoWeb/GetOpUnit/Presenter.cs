using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetOpUnit;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.GetOpUnit
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(GetOpUnitOutput output)
        {
            switch (output.Status)
            {
                case GetOpUnitStatus.OpUnitFound:
                    ViewModel = new CreatedResult("", new { opunit = output.Opunit, status = output.Status, mensagem = output.Mensagem });
                    break;
                case GetOpUnitStatus.OpUnitNotFound:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
