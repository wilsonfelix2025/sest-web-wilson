using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões;
using System.Net;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões
{
    public class Presenter
    {
        public IActionResult ViewModel { get; private set; }

        internal void Populate(CriarPerfilRelaçãoTensõesOutput output)
        {
            switch (output.Status)
            {
                case CriarPerfilRelaçãoTensõesStatus.PerfilCriado:
                    ViewModel = new OkObjectResult(new { output.Mensagem, output.Perfil });
                    break;
                case CriarPerfilRelaçãoTensõesStatus.PerfilNãoCriado:
                    ViewModel = new BadRequestObjectResult(output.Mensagem);
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
