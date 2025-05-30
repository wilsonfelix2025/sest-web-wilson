using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PoçoWeb.Webhook;

namespace SestWeb.Api.UseCases.PoçoWeb.Webhook
{
    ///<inheritdoc />
    public class Presenter
    {
        ///<inheritdoc />
        public IActionResult ViewModel { get; set; }

        ///<inheritdoc />
        public void Populate(WebhookOutput output)
        {
            switch (output.Status)
            {
                case WebhookStatus.Ok:
                    ViewModel = new CreatedResult("", new { status = output.Status, mensagem = output.Mensagem });
                    break;
                case WebhookStatus.NotOk:
                    ViewModel = new BadRequestObjectResult(new { mensagem = output.Mensagem, status = output.Status });
                    break;
                default:
                    ViewModel = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                    break;
            }
        }
    }
}
