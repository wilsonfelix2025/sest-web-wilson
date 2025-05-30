using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SestWeb.Domain.Webhook
{
    public class MoveWebhookConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            bool canConvert = objectType == typeof(MoveWebhookProperty);
            return canConvert;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            MoveWebhookProperty property = jo.SelectToken("event_data").ToObject<MoveWebhookProperty>();
            return property;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
