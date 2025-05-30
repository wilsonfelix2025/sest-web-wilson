using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PoçoWeb.Webhook
{
    public interface IWebhookUseCase
    {
        /// <summary>
        /// Execução de operações baseadas em eventos do WebKook.
        /// </summary>
        Task<WebhookOutput> Execute(string json);
    }
}
