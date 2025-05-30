using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetTree;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.GetTree
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(GetTreeOutput output)
        {
            switch (output.Status)
            {
                case GetTreeStatus.TreeFound:
                    ViewModel = new CreatedResult("", new { tree = output.Tree, status = output.Status, mensagem = output.Mensagem });
                    break;
                case GetTreeStatus.TreeNotFound:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
