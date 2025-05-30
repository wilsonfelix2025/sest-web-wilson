using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SestWeb.Domain.Webhook
{
    public class AddWebhookConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            bool canConvert = objectType == typeof(AddWebhookProperty);
            return canConvert;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            AddWebhookProperty property = jo.SelectToken("event_data").ToObject<AddWebhookProperty>();
            return property;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
