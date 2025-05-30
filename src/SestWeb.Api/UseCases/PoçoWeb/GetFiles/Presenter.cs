using System.Net;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.GetFiles;

namespace SestWeb.Api.UseCases.PoçoWeb.GetFiles
{
    public class Presenter
    {
        public IActionResult ViewModel { get; set; }

        public void Populate(GetFilesOutput output)
        {
            switch (output.Status)
            {
                case GetFilesStatus.BuscaComSucesso:
                    ViewModel = new OkObjectResult(new { listaArquivos = output.ListaArquivos, status = output.Status, mensagem = output.Mensagem });
                    break;
                case GetFilesStatus.BuscaSemSucesso:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
