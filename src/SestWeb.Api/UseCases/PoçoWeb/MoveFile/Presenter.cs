using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.MoveFile;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.MoveFile
{
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(MoveFileOutput output)
        {
            switch (output.Status)
            {
                case MoveFileStatus.FileMoved:
                    ViewModel = new OkObjectResult(new { fileName = output.FileName, wellName= output.WellName, status = output.Status, message = output.Mensagem });
                    break;
                case MoveFileStatus.FileNotMoved:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
