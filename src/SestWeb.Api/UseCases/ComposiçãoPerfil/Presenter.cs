
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ComposiçãoPerfil;
using System.Net;

namespace SestWeb.Api.UseCases.ComposiçãoPerfil
{
    public class Presenter
    {
        public IActionResult ViewModel { get; set; }

        internal void Populate(ComporPerfilOutput output)
        {
            switch (output.Status)
            {
                case ComporPerfilStatus.ComporPerfilComSucesso:
                    ViewModel = new OkObjectResult(new { mensagem = output.Mensagem, perfil = output.Perfil });
                    break;
                case ComporPerfilStatus.ComporPerfilComFalha:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                case ComporPerfilStatus.ComporPerfilComFalhaDeValidação:
                    ViewModel = new BadRequestObjectResult(new { output.Mensagem });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }

        }
    }
}
