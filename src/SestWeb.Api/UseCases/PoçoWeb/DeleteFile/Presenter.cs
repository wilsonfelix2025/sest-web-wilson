using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.DeleteFile;

namespace SestWeb.Api.UseCases.PoçoWeb.DeleteFile
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; private set; }

        ///<inheritdoc />
        public void Populate(DeleteFileOutput output)
        {
            switch (output.Status)
            {
                case DeleteFileStatus.FileDeleted:
                    ViewModel = new NoContentResult();
                    break;
                case DeleteFileStatus.FileNotFound:
                    ViewModel = new NotFoundObjectResult(new { mensagem = output.Mensagem });
                    break;
                case DeleteFileStatus.FileNotDeleted:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
