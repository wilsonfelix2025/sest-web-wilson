using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.DuplicateFile;
using System.Net;

namespace SestWeb.Api.UseCases.PoçoWeb.DuplicateFile
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(DuplicateFileOutput output)
        {
            switch (output.Status)
            {
                case DuplicateFileStatus.FileDuplicated:
                    ViewModel = new CreatedResult("", new { name = output.Name, id = output.Id, description = output.Description, comment = output.Comment, fileType = output.FileType, schema = output.Schema, well = output.Well, status = output.Status, mensagem = output.Mensagem });
                    break;
                case DuplicateFileStatus.FileNotDuplicated:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
