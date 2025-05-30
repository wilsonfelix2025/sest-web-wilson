using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SestWeb.Application.UseCases.PoçoWeb.CreateWell;
using SestWeb.Application.UseCases.PoçoWeb.Webhook;

namespace SestWeb.Api.UseCases.PoçoWeb.Webhook
{
    ///<inheritdoc />
    [Route("pocoweb/events")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly Presenter _presenter;
        private readonly IWebhookUseCase _useCase;

        ///<inheritdoc />
        public WebhookController(Presenter presenter, IWebhookUseCase useCase)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        /// <summary>
        /// Recebe eventos via webhook.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        public async Task<IActionResult> WebHookHandler()
        {
            Log.Information("Evento de webhook recebido.");

            WebhookOutput output;
            using (StreamReader reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
            {
                var json = await reader.ReadToEndAsync();
                Log.Information($"json: {json}");

                output = await _useCase.Execute(json);
            }

            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
