using System;
using System.Collections.Generic;
using System.Text;
using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoWeb.Webhook
{
    public class WebhookOutput : UseCaseOutput<WebhookStatus>
    {
        private WebhookOutput(WebhookStatus status)
        {
            Status = status;
        }

        public static WebhookOutput Ok()
        {
            return new WebhookOutput(WebhookStatus.Ok)
            {
                Mensagem = "Executado com sucesso."
            };
        }

        public static WebhookOutput NotOk(string message)
        {
            return new WebhookOutput(WebhookStatus.NotOk)
            {
                Mensagem = $"Falha ao executar. {message}"
            };
        }
    }
}
