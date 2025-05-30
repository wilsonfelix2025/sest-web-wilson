using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.RenameFile;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.RenameFile
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(RenameFileOutput output)
        {
            switch(output.Status)
            {
                case RenameFileStatus.FileRenamed:
                    ViewModel = new CreatedResult("", new { name = output.Name, description = output.Description, comment = output.Comment, fileType = output.FileType, schema = output.Schema });
                    break;
                case RenameFileStatus.FileNotRenamed:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
