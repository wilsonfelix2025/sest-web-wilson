using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SestWeb.Application.UseCases.PoçoWeb.Webhook;

namespace SestWeb.Application.Tests.UseCases.Webhook
{
    [TestFixture]
    public class hierarchySyncTest
    {
        [Test]
        public async Task DeveMontarAddCorretamente()
        {
            var json = @"
        {
            ""$schema"": ""http://json-schema.org/draft-04/schema#"",
            ""id"": ""http://jsonschema.net#"",
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""event_data"": { ""add"": [""OpUnit 1"", ""Campo 1"", ""Poço 1""]},
                ""event_type"": ""hierarchy_sync"",
                ""created_at"": ""2014-07-29T19:49:39Z""
            },
            ""required"": ['event_type', 'event_data', 'created_at']
        }";

            //var useCase = new WebhookUseCase();
            //var result = await useCase.Execute(json);
        }

        [Test]
        public async Task DeveMontarMoveCorretamente()
        {
            var json = @"
        {
            ""$schema"": ""http://json-schema.org/draft-04/schema#"",
            ""id"": ""http://jsonschema.net#"",
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
               ""event_data"": {
                    ""move"": [
                    [""OpUnit 1"", ""Campo 1"", ""Poço 1""],
                    [""OpUnit 1"", ""Campo 1"", ""Poço 1 renomeado""]]
                },
            ""event_type"": ""hierarchy_sync"",
                ""created_at"": ""2014-07-29T19:49:39Z""
            },
            ""required"": ['event_type', 'event_data', 'created_at']
        }";

            //var useCase = new WebhookUseCase();
            //var result = await useCase.Execute(json);
        }
    }
}
